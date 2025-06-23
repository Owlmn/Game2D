import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

/**
 * Write a description of class StartMenu here.
 * 
 * @author (your name) 
 * @version (a version number or a date)
 */
public class StartMenu extends World
{
    private Diff selectedDifficulty = Diff.MEDIUM;

    public StartMenu() {
        super(800, 600, 1);
        prepare();
    }
    
    public void setDifficulty(Diff difficulty) {
        selectedDifficulty = difficulty;
        Greenfoot.setWorld(this);
    }

    public Diff getDifficulty() {
        return selectedDifficulty;
    }

    private void prepare() {
        
        StartButton startButton = new StartButton(this);
        Controls controls = new Controls();
        Difficulty difficulty = new Difficulty(this);
        Exit exit = new Exit();
        addObject(startButton, 378, 189);
        addObject(controls, 376, 254);
        addObject(difficulty, 376, 320);
        addObject(exit, 376, 385);

        
        setBackground("menuBackground.jpg");
    }
}
