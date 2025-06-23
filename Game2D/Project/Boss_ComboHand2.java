import greenfoot.*;

public class Boss_ComboHand2 extends Actor {
    private int state = 0; // 0 - ожидание, 1 - атака, 2 - возвращение
    private int attackOrder = 0;
    private int waitTimer = 0;
    private int speed = 3;

    private int originalX, originalY;
    private int targetX, targetY; // зафиксированная цель
    
private boolean hasHitHero = false; // Флаг, была ли атака по герою
    private int collisionRadius = 30; // Радиус обнаружения героя
    private int damage = 200;
    
    public void set_damage(int coef){
        this.damage = damage + coef;
    }
    public void act() {
        if (getWorld() == null) return;

        if (state == 0) {
            waitTimer++;
            if (waitTimer >= attackOrder * 30) {
                Actor hero = getClosestHero();
                if (hero != null) {
                    targetX = hero.getX();
                    targetY = hero.getY();
                } else {
                    targetX = getX();
                    targetY = getY();
                }
                originalX = getX();
                originalY = getY();
                state = 1;
            }
        } else if (state == 1) {
            moveTowards(targetX, targetY);
            checkHeroCollision();
            if (distanceTo(targetX, targetY) < 10) {
                state = 2;
            }
        } else if (state == 2) {
            moveTowards(originalX, originalY);
            if (distanceTo(originalX, originalY) < 10) {
                getWorld().removeObject(this);
            }
        }
    }

    public void setDifficultyLevel(int level) {
    // Базовая скорость: 3, увеличивается с уровнем
    this.speed = 3 + (level - 1) * 2; // Пример: уровень 1 — скорость 3, уровень 2 — 5, уровень 3 — 7
}

    public void setAttackOrder(int order) {
        this.attackOrder = order;
    }

    private void moveTowards(int tx, int ty) {
        int dx = tx - getX();
        int dy = ty - getY();
        double length = Math.sqrt(dx * dx + dy * dy);

        if (length != 0) {
            dx = (int)(dx / length * speed);
            dy = (int)(dy / length * speed);
        }

        setLocation(getX() + dx, getY() + dy);
    }

    private double distanceTo(int x, int y) {
        int dx = x - getX();
        int dy = y - getY();
        return Math.sqrt(dx * dx + dy * dy);
    }

    private Actor getClosestHero() {
        for (Object obj : getWorld().getObjects(Hero.class)) {
            return (Actor)obj; // берём первого героя
        }
        return null;
    }
    
    private void checkHeroCollision() {
    if (getWorld() == null || hasHitHero) return;

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
