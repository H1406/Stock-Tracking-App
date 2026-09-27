using SplashKitSDK;
using static SplashKitSDK.SplashKit;
using System;
using System.Drawing;
public class TextInput:Item{
    private static  SplashKitSDK.Rectangle rect;
    private string _inputTerm;
    private bool _isActivate;

    public TextInput(float x, float y):base(x,y){ 
        Width = 50;
        Height = 22;
        rect = RectangleFrom(X, Y,Width,Height);
        _inputTerm = "";
    }
    public TextInput(float x, float y, int width, int height):base(x,y){
        Width = width;
        Height = height;
        rect = RectangleFrom(X, Y,Width,Height);
    }
    public override void Draw(){
        FontLoader.EnsureLoaded();
        DrawRectangle(ColorBlack(), X, Y, Width, Height);
        FillRectangle(ColorWhite(), X+1, Y+1, Width-2, Height-2);
        DrawText(_inputTerm,ColorBlack(),"Arial",20, X+5, Y+5);
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
               _inputTerm = TextInput();
            }  
        }else _inputTerm = "";
    }
    //Activate and Deactivate the text input
    public void Activate(){
        _isActivate = true;
    }
    public void Deactivate(){
        _isActivate = false;
        EndReadingText();
    }
    public override bool IsClicked(float mousex, float mousey)
    {
        if (mousex >= X && mousex <= Width + X && mousey >= Y && mousey <= Y + Height){
            _isActivate = true;
            return true;
        }
        return false;
    }
    public string InputTerm{get=> _inputTerm;set=> _inputTerm = value;}
    public bool IsActivate{get=> _isActivate; set=> _isActivate = value;}
}