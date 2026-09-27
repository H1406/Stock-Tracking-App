using System;
using System.Drawing;
using System.IO;
using static SplashKitSDK.SplashKit;
using SplashKitSDK;

namespace StockApp
{
    public class Program
    
    {
        private static SplashKitSDK.Color Background = RGBColor(64, 64, 64);
        public static void Main(){
            Directory.SetCurrentDirectory(System.AppContext.BaseDirectory);
            FontLoader.EnsureLoaded();
            Page home = PageFactory.CreatePage("home");
            Page follow = PageFactory.CreatePage("follow");
            Page detail = PageFactory.CreatePage("detail");
            Page wallet = PageFactory.CreatePage("wallet");
            AppContext context = new AppContext();
            context.SetState(new HomeState((HomePage)home,(FollowPage) follow,(DetailPage)detail));

            OpenWindow("Stock App", 800, 600);
            do{
                ClearScreen(Background);
                ProcessEvents();

                // Handle input and draw the current state
                context.HandleInput();
                context.Update();
                context.Draw();
                // Check if a state transition is needed
                Page_type nextState = context.GetNextState();
                StockItem item = context.GetItem();
                if (nextState != Page_type.home) // Transition to a new state
                {
                    switch (nextState)
                    {
                        case Page_type.following:
                            context.SetState(new FollowState((FollowPage) follow,(HomePage) home,(DetailPage) detail));
                            break;
                        case Page_type.detail:
                            context.SetState(new DetailState((DetailPage)detail,new StockItem(item.Name, item.X, item.Y, item.High, item.Low, item.Open, item.Current,item.Quantity),(FollowPage)follow,(WalletPage)wallet));
                            break;
                        case Page_type.wallet:
                            context.SetState(new WalletState((WalletPage)wallet, (DetailPage)detail));
                            break;
                    }
                }
                else{
                    context.SetState(new HomeState((HomePage)home,(FollowPage) follow,(DetailPage)detail));
                }
                RefreshScreen();
            } while (!QuitRequested());
        }
    }
}
