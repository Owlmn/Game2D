import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

/**
 * Write a description of class StartButton here.
 * 
 * @author (your name) 
 * @version (a version number or a date)
 */
public class StartButton extends Actor
{
    private StartMenu menu;
    private GreenfootImage normalImage = new GreenfootImage("start.png");
    private GreenfootImage hoverImage = new GreenfootImage("start_hover.png");
    
    public StartButton(StartMenu menu) {
        this.menu = menu;
        setImage("start.png"); // изображение кнопки
    }

    public void act() {
        if (Greenfoot.mouseMoved(this)) {
            setImage(hoverImage);
        }

        if (Greenfoot.mouseMoved(null) && !Greenfoot.mouseMoved(this)) {
            setImage(normalImage);
        }
        
        if (Greenfoot.mouseClicked(this)) {
            Diff selected = menu.getDifficulty();
            Greenfoot.setWorld(new MAP(selected)); // запуск основного мира
        }
    }
}
