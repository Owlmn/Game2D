import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

/**
 * Write a description of class DifficultyButton here.
 * 
 * @author (your name) 
 * @version (a version number or a date)
 */
public class DifficultyButton extends Actor
{
    private Diff difficulty;
    private StartMenu menu;
    private GreenfootImage normalImage;
    private GreenfootImage hoverImage;

    public DifficultyButton(Diff difficulty, String label, StartMenu menu) {
        this.difficulty = difficulty;
        this.menu = menu;

        normalImage = new GreenfootImage(200, 50);
        normalImage.setColor(Color.LIGHT_GRAY);
        normalImage.fillRect(0, 0, 200, 50);
        normalImage.setColor(Color.BLACK);
        normalImage.setFont(new Font("Arial", true, false, 22));
        normalImage.drawString(label, 50, 32);

        hoverImage = new GreenfootImage(200, 50);
        hoverImage.setColor(Color.RED);
        hoverImage.fillRect(0, 0, 200, 50);
        hoverImage.setColor(Color.BLACK);
        hoverImage.setFont(new Font("Arial", true, false, 22));
        hoverImage.drawString(label, 50, 32);

        setImage(normalImage);
    }

    public void act() {
        if (Greenfoot.mouseMoved(this)) {
            setImage(hoverImage);
        } else if (Greenfoot.mouseMoved(null)) {
            setImage(normalImage);
        }

        if (Greenfoot.mouseClicked(this)) {
            menu.setDifficulty(difficulty);
        }
    }
}
