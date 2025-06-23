import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

/**
 * Write a description of class Controls here.
 * 
 * @author (your name) 
 * @version (a version number or a date)
 */
public class Controls extends Actor
{
    private GreenfootImage normalImage = new GreenfootImage("controls.png");
    private GreenfootImage hoverImage = new GreenfootImage("controls_hover.png");
    
    public Controls() {
        setImage("controls.png"); // изображение кнопки
    }

    public void act() {
        if (Greenfoot.mouseMoved(this)) {
            setImage(hoverImage);
        }

        if (Greenfoot.mouseMoved(null) && !Greenfoot.mouseMoved(this)) {
            setImage(normalImage);
        }
        
        if (Greenfoot.mouseClicked(this)) {
            Greenfoot.setWorld(new Handling()); // запуск основного мира
        }
    }
}
