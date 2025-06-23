import greenfoot.*;  // (World, Actor, GreenfootImage, Greenfoot and MouseInfo)

public class MAP5 extends MyWorld {
    private Hero hero;

    private double koef = 1;
    public MAP5(Diff difficulty,Hero h) {
        super(difficulty);
        this.hero = h;
        setHero(h);
        koef = coef;
        addObject(hero,  getWidth() / 2, getHeight() / 2); 
        prepare();
        
    }

    private void prepare() {
        // Добавление босса
        Boss boss = new Boss(coef,hero);
        int bossX = getWidth() / 2;
        int bossY = getHeight() / 8;
        addObject(boss, bossX, bossY);

    }

    public void act() {
        if (hero!=null){
           checkKeys(); 
        }
        
    }

    private void checkKeys() {
        int speed = 4;

        if (Greenfoot.isKeyDown("w")) hero.move(0, -speed);
        if (Greenfoot.isKeyDown("s")) hero.move(0, speed);
        if (Greenfoot.isKeyDown("a")) hero.move(-speed, 0);
        if (Greenfoot.isKeyDown("d")) hero.move(speed, 0);
        if (Greenfoot.mousePressed(null)) hero.shoot();
    }
}
