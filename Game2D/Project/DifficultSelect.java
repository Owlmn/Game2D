import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)


public class DifficultSelect extends World
{

    private StartMenu menu;
    public DifficultSelect(StartMenu menu) {
        super(800, 600, 1); // или размеры твоего игрового мира
        this.menu = menu;
        prepare();
    }
    
    public void act(){
        if (Greenfoot.isKeyDown("escape")){
            Greenfoot.setWorld(new StartMenu());
        }
    }
    
    private void prepare() {
        addObject(new DifficultyButton(Diff.EASY, "Лёгкий",menu), getWidth()/2-100, 100);
        addObject(new DifficultyButton(Diff.MEDIUM, "Средний",menu), getWidth()/2-100, 220);
        addObject(new DifficultyButton(Diff.HARD, "Сложный",menu), getWidth()/2-100, 350);
        addObject(new DifficultyButton(Diff.HARDCORE, "Хардкор",menu), getWidth()/2-100, 470);
    }
}
