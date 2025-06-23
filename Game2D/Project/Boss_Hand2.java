import greenfoot.*;

public class Boss_Hand2 extends Actor {
    private int direction = 1;       // +1 или -1
    private int moveType = 0;        // 0: вертикаль, 1: горизонталь, 2: диагональ

    private boolean randomBulletMode = false;
    private int shootTimer = 600;     // Время жизни в кадрах
    private int cooldown = 30;        // Интервал между выстрелами
    private int cooldownTimer = 0;
     private boolean hasHitHero = false; // Флаг, была ли атака по герою
    private int collisionRadius = 30; // Радиус обнаружения героя
    private int damage = 100; 
    private int speed = 1;           // Скорость движения
public void set_damage(int coef){
        this.damage = damage + coef;
    }
     public void act() {
        if (getWorld() == null) return;
        movePattern();
        checkOutOfBounds();
    }

    public void setSpeed(int speed) {
        this.speed = speed;
    }

    public void setMoveType(int type) {
        this.moveType = type;
    }

    public void setDirection(int dir) {
        this.direction = dir;
    }

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
           case 3: 
                setLocation(getX() - direction * speed, getY() + direction * speed);
                break;
        }
    }

   private void checkOutOfBounds() {
    World world = getWorld();
    if (world == null) return;

    int x = getX();
    int y = getY();
    int w = world.getWidth();
    int h = world.getHeight();

    switch (moveType) {
        case 0: // вертикаль
            if (y <= 0 || y >= h - 1) world.removeObject(this);
            break;
        case 1: // горизонталь
            if (x <= 0 || x >= w - 1) world.removeObject(this);
            break;
        case 2:
            if (y <= 0 || y >= h - 1) world.removeObject(this);
            break;
        case 3: 
            if (y <= 0 || y >= h - 1) world.removeObject(this);
            break;
    }
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
