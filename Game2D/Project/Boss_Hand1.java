import greenfoot.*;

public class Boss_Hand1 extends Actor {
    private int direction = 1;       // Направление движения (+1 или -1)
    private int moveType = 0;        // Тип движения (0: вертикаль, 1: горизонталь, 2: диагональ1, 3: диагональ2)
    private int speed = 1;           // Скорость движения
    private boolean hasHitHero = false; // Флаг, была ли атака по герою
    private int collisionRadius = 30; // Радиус обнаружения героя
    private int damage = 100; 
     
    public void set_damage(int coef){
        this.damage = damage + coef;
    } 
    
    public void act() {
        if (getWorld() == null) return; // Если рука удалена - выходим
        
        movePattern(); // Двигаем руку
        
        // Проверяем выход за границы (если true - рука удалена)
        if (checkOutOfBounds()) return;
        
        // Если ещё не били героя - проверяем коллизию
        if (!hasHitHero) {
            checkHeroCollision();
        }
    }

    // Установка скорости
    public void setSpeed(int speed) {
        this.speed = speed;
    }

    // Установка типа движения
    public void setMoveType(int type) {
        this.moveType = type;
    }

    // Установка направления
    public void setDirection(int dir) {
        this.direction = dir;
    }

    // Паттерны движения
    private void movePattern() {
        switch (moveType) {
            case 0: // вертикаль
                setLocation(getX(), getY() + direction * speed);
                break;
            case 1: // горизонталь
                setLocation(getX() + direction * speed, getY());
                break;
            case 2: // диагональ (из левого верхнего в правый нижний)
                setLocation(getX() + direction * speed, getY() + direction * speed);
                break;
            case 3: // диагональ (из правого верхнего в левый нижний)
                setLocation(getX() - direction * speed, getY() + direction * speed);
                break;
        }
    }

    // Проверка выхода за границы мира
    private boolean checkOutOfBounds() {
        World world = getWorld();
        if (world == null) return true;

        int x = getX();
        int y = getY();
        int width = world.getWidth();
        int height = world.getHeight();

        boolean outOfBounds = false;
        
        switch (moveType) {
            case 0: // вертикаль
                outOfBounds = (y <= 0 || y >= height - 1);
                break;
            case 1: // горизонталь
                outOfBounds = (x <= 0 || x >= width - 1);
                break;
            case 2: // диагональ1
            case 3: // диагональ2
                outOfBounds = (y <= 0 || y >= height - 1);
                break;
        }
        
        if (outOfBounds) {
            world.removeObject(this);
            return true;
        }
        return false;
    }

  private void checkHeroCollision() {
    if (getWorld() == null) return;

    java.util.List<Hero> heroes = getWorld().getObjects(Hero.class);
    for (Hero hero : heroes) {
        if (intersects(hero)) {
            hero.takeDamage(damage);
            hasHitHero = true;
            return;
        }
    }
}


    // Поиск ближайшего героя в радиусе
    private Hero getNearestHero() {
        if (getWorld() == null) return null;
        
        // Ищем всех героев в заданном радиусе
        java.util.List<Hero> heroes = getObjectsInRange(collisionRadius, Hero.class);
        if (!heroes.isEmpty()) {
            return heroes.get(0); // Возвращаем первого найденного героя
        }
        return null;
    }
}
