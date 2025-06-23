import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

/**
 * Write a description of class Handling here.
 * 
 * @author (your name) 
 * @version (a version number or a date)
 */
public class Handling extends World
{

    /**
     * Constructor for objects of class Handling.
     * 
     */
    public Handling()
    {    
        super(800, 600, 1);
        //prepare();
    }
    
    public void act(){
        if (Greenfoot.isKeyDown("escape")){
            Greenfoot.setWorld(new StartMenu());
        }
    }
    
    private void prepare() {
        
    }
}
