import greenfoot.*;

public class Poison extends Actor {
    private int speed = 5;

    public Poison() {
        GreenfootImage img = getImage();
        img.scale(img.getWidth() / 3, img.getHeight() / 3);
        setImage(img);
    }

    public void act() {
        move(speed);

        // Проверка на попадание в игрока
        if (isTouching(Hero.class)) {
            Hero hero = (Hero) getOneIntersectingObject(Hero.class);
            if (hero != null) {
                hero.takeDamage(15); // Урон ядом
            }
            getWorld().removeObject(this);
            return;
        }

        // Проверка на столкновение со стенами
        if (isTouching(Wall_gorizont.class) || isTouching(Wall_vertical.class) || isTouching(Cube.class)) {
            getWorld().removeObject(this);
            return;
        }

        // Удаление на краю экрана
        if (isAtEdge()) {
            getWorld().removeObject(this);
        }
    }
}
