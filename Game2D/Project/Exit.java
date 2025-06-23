import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

/**
 * Write a description of class Exit here.
 * 
 * @author (your name) 
 * @version (a version number or a date)
 */
public class Exit extends Actor
{
    private GreenfootImage normalImage = new GreenfootImage("exit.png");
    private GreenfootImage hoverImage = new GreenfootImage("exit_hover.png");
    
    public Exit() {
        setImage("exit.png"); // изображение кнопки
    }

    public void act() {
        if (Greenfoot.mouseMoved(this)) {
            setImage(hoverImage);
        }

        if (Greenfoot.mouseMoved(null) && !Greenfoot.mouseMoved(this)) {
            setImage(normalImage);
        }
        
        if (Greenfoot.mouseClicked(this)) {
            Greenfoot.stop(); // остановка
        }
    }
}
