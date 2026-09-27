using System.Drawing;
using SplashKitSDK;
using static SplashKitSDK.SplashKit;
using System.Threading.Tasks;
using System.Net.Http;

public class SearchBar:Item{
    private string _searchTerm;
    private bool _isActivate;
    private SplashKitSDK.Rectangle rect;
    public SearchBar( float x, float y):base(x,y){
        Width = 200;
        Height = 22;
        rect = RectangleFrom(X, Y,Width,Height);
    }

    public override void Draw()
    {
        FontLoader.EnsureLoaded();
        DrawRectangle(ColorBlack(), X, Y, Width, Height);
        FillRectangle(ColorWhite(), X+1, Y+1, Width-2, Height-2);
        DrawText(_searchTerm,ColorBlack(),"Arial",20, X+5, Y+5);
        if (_isActivate){
            HandleUserInput();
        }
    }
    public void HandleUserInput(){
        FontLoader.EnsureLoaded();
        if (_isActivate){
            if(!ReadingText()){
                StartReadingText(rect);
            }
            else if (ReadingText()){
               _searchTerm = TextInput();
            }  
        }
    }
    public override bool IsClicked(float mousex, float mousey)
    {
        if (mousex >= X && mousex <= Width + X && mousey >= Y && mousey <= Y + Height){
            _isActivate = true;
            return true;
        }
        return false;
    }
    public string SearchTerm{get=>_searchTerm;}
}