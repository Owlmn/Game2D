import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

/**
 * Write a description of class Difficulty here.
 * 
 * @author (your name) 
 * @version (a version number or a date)
 */
public class Difficulty extends Actor
{
    private StartMenu menu;
    private GreenfootImage normalImage = new GreenfootImage("difficulty.png");
    private GreenfootImage hoverImage = new GreenfootImage("difficulty_hover.png");
    
    public Difficulty(StartMenu menu) {
        setImage("difficulty.png"); // изображение кнопки
        this.menu = menu;
    }

    public void act() {
        if (Greenfoot.mouseMoved(this)) {
            setImage(hoverImage);
        }

        if (Greenfoot.mouseMoved(null) && !Greenfoot.mouseMoved(this)) {
            setImage(normalImage);
        }
        
        if (Greenfoot.mouseClicked(this)) {
            Greenfoot.setWorld(new DifficultSelect(menu));
        }
    }
}
