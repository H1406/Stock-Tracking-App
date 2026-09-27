using System;
using Microsoft.Data.Sqlite;

public class DatabaseManager
{
    private List<StockItem> _stocks = new List<StockItem>();
    private StockFetcher _fetcher = new StockFetcher();
    private static readonly string DatabaseFile = AppPaths.DatabaseFile;
    public DatabaseManager()
    {
        using(var connection = new SqliteConnection($"Data Source={DatabaseFile}")){
            connection.Open();
            string tableCommand = @"CREATE TABLE IF NOT EXISTS StockData (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                symbol TEXT PRIMARY KEY NOT NULL,
                amount INTEGER NOT NULL
                )";
            using(var command = new SqliteCommand(tableCommand, connection)){
                command.ExecuteNonQuery();
            }
        }
    }
    public void SaveStock(StockItem stock){
        //Save stock to the database
    try{
        using (var connection = new SqliteConnection($"Data Source={DatabaseFile}"))
        {
            connection.Open();
            string insertCommand = @"INSERT INTO StockData (symbol) VALUES (@symbol)";
            using (var command = new SqliteCommand(insertCommand, connection))
            {
                command.Parameters.AddWithValue("@symbol", stock.Name);
                command.ExecuteNonQuery();
            }
        }
    }
    catch (Exception ex){
        Console.WriteLine($"Error saving stock: {ex.Message}");
        }
    }       

    public void RemoveStock(StockItem stock){
        //Remove stock from the database
        using(var connection = new SqliteConnection($"Data Source={DatabaseFile}")){
            connection.Open();
            string deleteCommand = @"DELETE FROM StockData WHERE symbol = @symbol AND amount = 0";
            using(var command = new SqliteCommand(deleteCommand, connection)){
                command.Parameters.AddWithValue("@symbol", stock.Name);
                command.ExecuteNonQuery();
            }
        }
    }
    public async Task LoadData()
    {
        //Load following stocks from the database
        using(var connection = new SqliteConnection($"Data Source={DatabaseFile}"))
        {
            connection.Open();
            string selectCommand = @"SELECT symbol FROM StockData";
            using(var command = new SqliteCommand(selectCommand, connection))
            {
                using(var reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        string symbol = reader.GetString(0);
                        // Await the fetch for each stock symbol
                        await _fetcher.FetchStockData(symbol);
                        // Check if data was found for this symbol
                        if (_fetcher.ItemFound != null)
                        {
                            // Create a new StockItem with proper property access
                            StockItem stock = new StockItem(
                                _fetcher.ItemFound.Name, 
                                _fetcher.ItemFound.X, 
                                _fetcher.ItemFound.Y, 
                                _fetcher.ItemFound.High, 
                                _fetcher.ItemFound.Low, 
                                _fetcher.ItemFound.Open, 
                                _fetcher.ItemFound.Current
                                );
                            
                            _stocks.Add(stock);
                        }
                        else
                        {
                            Console.WriteLine($"No data found for {symbol}");
                        }
                    }
                }
            }
        }
    }
    
    public async Task GetWallet(){
        /// <summary>
        /// Retrieves all items from the database with amounts greater than 0
        /// and loads them into the _stocks list.
        /// </summary>
        using(var connection = new SqliteConnection($"Data Source={DatabaseFile}")){
            connection.Open();
            string selectCommand = @"SELECT symbol,amount FROM StockData WHERE amount > 0";
            using(var command = new SqliteCommand(selectCommand, connection)){
                using(var reader = command.ExecuteReader()){
                    while(reader.Read()){
                        string symbol = reader.GetString(0);
                        int amountIndex = reader.GetOrdinal("amount");
                        int amount = reader.GetInt32(amountIndex);
                        await _fetcher.FetchStockData(symbol);
                        if(_fetcher.ItemFound != null){
                            StockItem stock = new StockItem(
                                _fetcher.ItemFound.Name, 
                                _fetcher.ItemFound.X, 
                                _fetcher.ItemFound.Y, 
                                _fetcher.ItemFound.High, 
                                _fetcher.ItemFound.Low, 
                                _fetcher.ItemFound.Open, 
                                _fetcher.ItemFound.Current, 
                                amount);
                            _stocks.Add(stock);
                        }
                    }
                }
            }
        }
    }
    public void SaveWallet(StockItem stock){
        //Update the amount of the stock in the database
        using(var connection = new SqliteConnection($"Data Source={DatabaseFile}")){
            connection.Open();
            string updateCommand = @"Update StockData SET amount = @amount WHERE symbol = @symbol";
            using(var command = new SqliteCommand(updateCommand, connection)){
                command.Parameters.AddWithValue("@amount", stock.Quantity);
                command.Parameters.AddWithValue("@symbol", stock.Name);
                command.ExecuteNonQuery();
            }
        }
    }
    public StockItem LoadStock(string symbol){
        //Load a stock from the database
        _fetcher.FetchStockData(symbol).Wait();
        StockItem stock = new StockItem(
                                _fetcher.ItemFound.Name, 
                                _fetcher.ItemFound.X, 
                                _fetcher.ItemFound.Y, 
                                _fetcher.ItemFound.High, 
                                _fetcher.ItemFound.Low, 
                                _fetcher.ItemFound.Open, 
                                _fetcher.ItemFound.Current);
        return stock;
    }
    public List<StockItem> Stocks{get=>_stocks;}
}