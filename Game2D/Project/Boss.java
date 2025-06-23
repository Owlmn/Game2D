import greenfoot.*;
import java.util.*;

public class Boss extends Actor {
    private int floatOffset = 0;
    private int direction = 1;
    private double koef=1;
    private int maxHealth = (int)(1000 * koef);
    private int health = maxHealth;
    private long lastAttackTime = 0;
    private int difficultyLevel = 1;
    private long startTime;
    private long lastDifficultyCheck;
    private Queue<Integer> attackQueue = new LinkedList<>();
    private double attackSpeedMultiplier = 1.0;
    private int baseAttackCooldown = 5000;
    private GreenfootImage healthBar;
    private int healthBarWidth = 400;
    private int healthBarHeight = 25;
    private HealthBar bar;
    private Hero hero;
    
    public Boss(double coef, Hero h) {
        startTime = System.currentTimeMillis();
        lastDifficultyCheck = startTime;
        initializeAttackQueue();
        this.koef = coef;
        maxHealth = (int)(maxHealth * koef);
        health = maxHealth;
        this.attackSpeedMultiplier = (double)(1.0 + coef);
        this.baseAttackCooldown = (int)(baseAttackCooldown / (1+coef));
        this.hero = h;
        
        healthBar = new GreenfootImage(healthBarWidth, healthBarHeight);
        updateHealthBar();
        
    }

    public void act() {
        if (getWorld() == null || hero==null) return;

        if (bar == null) {
            bar = new HealthBar(healthBar);
            getWorld().addObject(bar, getX(), getY() - 80);
        }

        updateDifficulty();
        updateAttackSpeed();
        floatUpDown();

        long currentTime = System.currentTimeMillis();
        int currentCooldown = (int)(baseAttackCooldown / attackSpeedMultiplier);

        if (currentTime - lastAttackTime > currentCooldown) {
            startNextAttack();
            lastAttackTime = currentTime;
        }

        
        if (bar != null) {
            bar.setLocation(getX(), getY() - 80);
        }
    }
    
    private void updateHealthBar() {
        healthBar.clear();
        healthBar.setColor(Color.BLACK);
        healthBar.fill();
        healthBar.setColor(Color.GRAY);
        healthBar.fillRect(1, 1, healthBarWidth - 2, healthBarHeight - 2);

        int currentWidth = (int)((double)health / maxHealth * (healthBarWidth - 4));
        healthBar.setColor(Color.RED);
        healthBar.fillRect(2, 2, currentWidth, healthBarHeight - 4);

        if (bar != null) {
            bar.setImage(healthBar);
        }
    }
    
    private void updateAttackSpeed() {
        long elapsed = (System.currentTimeMillis() - startTime) / 1000;
        attackSpeedMultiplier = 1.0 + (elapsed / 15.0) * 0.5;
        attackSpeedMultiplier = Math.min(attackSpeedMultiplier, 3.0);
    }
    
    private void initializeAttackQueue() {
        attackQueue.clear();
        attackQueue.add(0); // Вертикальная
        attackQueue.add(1); // Горизонтальная
        attackQueue.add(2); // Диагональная
        attackQueue.add(3); // Комбо
        attackQueue.add(4); // Пулемет
    }
    
    private void startNextAttack() {
        if (attackQueue.isEmpty()) {
            initializeAttackQueue();
        }
        
        int nextAttack = attackQueue.poll();
        
        switch (nextAttack) {
            case 0: attackVertical(difficultyLevel); break;
            case 1: attackHorizontal(difficultyLevel); break;
            case 2: attackDiagonal(difficultyLevel); break;
            case 3: attackSpecialCombo(); break;
            case 4: attackBulletSpam(); break;
        }
        
        attackQueue.add(nextAttack);
    }
    
    private void updateDifficulty() {
        long currentTime = System.currentTimeMillis();
        long elapsed = (currentTime - startTime) / 1000;
        
        if ((currentTime - lastDifficultyCheck) > 10000) { 
            lastDifficultyCheck = currentTime;
            
            if (elapsed > 60) {
                difficultyLevel = 3;
            } else if (elapsed > 30) {
                difficultyLevel = 2;
            } else {
                difficultyLevel = 1;
            }
        }
    }

    private void floatUpDown() {
        setLocation(getX(), getY() + direction);
        floatOffset += direction;

        if (Math.abs(floatOffset) >= 10) {
            direction *= -1;
        }
    }

    private void attackVertical(int level) {
    World world = getWorld();
    if (world == null) return;

    int worldWidth = world.getWidth();
    int worldHeight = world.getHeight();

  
    int[][] spawnPositions = {
        {worldWidth / 5, worldHeight - 1, -1},  // слева, сверху вниз (направление -1)
        {2 * worldWidth / 5, 0, 1},            // левее центра, снизу вверх (направление +1)
        {3 * worldWidth / 5, worldHeight - 1, -1}, // правее центра, сверху вниз (направление -1)
        {4 * worldWidth / 5, 0, 1}              // справа, снизу вверх (направление +1)
    };

    List<int[]> spawnList = Arrays.asList(spawnPositions);
    Collections.shuffle(spawnList); // перемешиваем позиции

    int handsToSpawn = 2 + new java.util.Random().nextInt(2); 
    java.util.Random rand = new java.util.Random();

    for (int i = 0; i < handsToSpawn; i++) {
        int[] pos = spawnList.get(i);
        
        // Случайный выбор между Boss_Hand1 и Boss_Hand2
        Actor hand;
        if (rand.nextBoolean()) {
            hand = new Boss_Hand1();
            
        } else {
            hand = new Boss_Hand2();
        }
        
       
        if (hand instanceof Boss_Hand1) {
            Boss_Hand1 hand1 = (Boss_Hand1) hand;
            hand1.set_damage(100 + (int)(koef * 20));
            hand1.setMoveType(0); // диагональное движение
            hand1.setDirection(pos[2]);
            hand1.setSpeed(1 + level);
            // Поворот руки: -45° если вверх, +45° если вниз
            hand1.setRotation(pos[2] == 1 ? -45 : 45);
        } else if (hand instanceof Boss_Hand2) {
            Boss_Hand2 hand2 = (Boss_Hand2) hand;
            hand2.set_damage(100 + (int)(koef * 20));
            hand2.setMoveType(0); 
            hand2.setDirection(pos[2]);
            hand2.setSpeed(1 + level);
            // Поворот руки: -45° если вверх, +45° если вниз
            hand2.setRotation(pos[2] == 1 ? -45 : 45);
        }

        world.addObject(hand, pos[0], pos[1]);
    }
}

    private void attackHorizontal(int level) {
       World world = getWorld();
        if (world == null) return;

        int[] ySlots = {
            getWorld().getHeight() / 4,
            getWorld().getHeight() / 2,
            3 * getWorld().getHeight() / 4
        };

        List<Integer> indices = Arrays.asList(0, 1, 2);
        Collections.shuffle(indices);

        for (int i = 0; i < 2; i++) {
            int yIndex = indices.get(i);
            int y = ySlots[yIndex];

            if (Greenfoot.getRandomNumber(2) == 0) {
                Boss_Hand1 hand = new Boss_Hand1();
                hand.set_damage(100 + (int)(koef * 20));
                hand.setDirection(-1);
                hand.setMoveType(1);
                hand.setSpeed(1 + level);
                hand.setRotation(45);
                world.addObject(hand, world.getWidth() - 1, y);
            } else {
                Boss_Hand2 hand = new Boss_Hand2();
                hand.set_damage(100 + (int)(koef * 20));
                hand.setDirection(1);
                hand.setMoveType(1);
                hand.setSpeed(1 + level);
                hand.setRotation(-45);
                world.addObject(hand, 0, y);
            }
        } 
    }

    private void attackDiagonal(int level) {
        World world = getWorld();
        if (world == null) return;

        int width = world.getWidth();
        int height = world.getHeight();

        for (int i = 0; i < level; i++) {
            Boss_Hand2 hand2 = new Boss_Hand2();
            hand2.set_damage(100 + (int)(koef * 20));
            hand2.setDirection(1);
            hand2.setMoveType(2);
            world.addObject(hand2, i * 50, i * 50);

            Boss_Hand1 hand1 = new Boss_Hand1();
            hand1.set_damage(100 + (int)(koef * 20));
            hand1.setDirection(-1);
            hand1.setMoveType(2);
            hand1.setRotation(90);
            world.addObject(hand1, width - 1 - i * 50, height - 1 - i * 50);

            Boss_Hand1 hand3 = new Boss_Hand1();
            hand3.set_damage(100 + (int)(koef * 20));
            hand3.setDirection(-1);
            hand3.setMoveType(3);
            hand3.setRotation(180);
            world.addObject(hand3, 0 + i * 50, height - 1 - i * 50);

            Boss_Hand2 hand4 = new Boss_Hand2();
            hand4.set_damage(100 + (int)(koef * 20));
            hand4.setDirection(1);
            hand4.setMoveType(3);
            hand4.setRotation(90);
            world.addObject(hand4, width - 1 - i * 50, 0 + i * 50);
        }
    }

    private void attackSpecialCombo() {
    World world = getWorld();
    if (world == null) return;

    for (int i = 0; i < difficultyLevel; i++) {
       
        int offsetX = i * 50;
        int offsetY = 20 + i * 10;

        // Левая рука (ComboHand1)
        Boss_ComboHand1 hand = new Boss_ComboHand1();
        hand.set_damage(200 + (int)(koef * 20));
        hand.setAttackOrder(i);
        hand.setDifficultyLevel(difficultyLevel);
        world.addObject(
            hand,
            getX() + 150 + offsetX,
            getY() + offsetY
        );

        // Правая рука (ComboHand2)
        Boss_ComboHand2 rightHand = new Boss_ComboHand2();
        hand.set_damage(200 + (int)(koef * 20));
        rightHand.setAttackOrder(i);
        rightHand.setDifficultyLevel(difficultyLevel);
        world.addObject(
            rightHand,
            getX() - 150 - offsetX,
            getY() + offsetY
        );
    }
}

    private void attackBulletSpam() {
    World world = getWorld();
    if (world == null) return;

    int pattern = Greenfoot.getRandomNumber(3) + 1; 
    int offset = 400;

    switch (pattern) {
        case 1:
            // Обычное расположение по горизонтали
            ShooterHand1 hand1 = new ShooterHand1();
            hand1.set_coef(koef);
            
            world.addObject(hand1, getX() + offset, getY());

            ShooterHand2 hand2 = new ShooterHand2();
             hand2.set_coef(koef);
            world.addObject(hand2, getX() - offset, getY());
            break;

        case 2:
            // Зеркальное расположение, под диагональю, с поворотом на 90
            ShooterHand1 diagHand1 = new ShooterHand1();
            diagHand1.set_coef(koef);
            diagHand1.setRotation(180);
            world.addObject(diagHand1, getX() - offset, getY() + offset);

            ShooterHand2 diagHand2 = new ShooterHand2();
            diagHand2.set_coef(koef);
            diagHand2.setRotation(180);
            world.addObject(diagHand2, getX() + offset, getY() + offset);
            break;

        case 3:
            // Квадрат из 4-х рук
           ShooterHand2 topLeft = new ShooterHand2();
             topLeft.set_coef(koef);
        world.addObject(topLeft, getX() - offset, getY());

        ShooterHand1 topRight = new ShooterHand1();
         topRight.set_coef(koef);
        world.addObject(topRight, getX() + offset, getY());

        ShooterHand1 bottomLeft = new ShooterHand1();
        bottomLeft.set_coef(koef);
        bottomLeft.setRotation(180);
        world.addObject(bottomLeft, getX() - offset, getY() + offset + 50);

        ShooterHand2 bottomRight = new ShooterHand2();
        bottomRight.set_coef(koef);
        bottomRight.setRotation(180);
        world.addObject(bottomRight, getX() + offset, getY() + offset + 50);
            break;
    }
    }

    public void takeDamage(int amount) {
        health -= amount * difficultyLevel;
        if (health < 0) health = 0;
        updateHealthBar();

        if (health <= 0 && getWorld() != null) {
            getWorld().addObject(new Win() , getWorld().getWidth()/2,getWorld().getHeight()/2);
            getWorld().removeObject(this);
            Greenfoot.stop();
        }
    }
    
    public int getDifficultyLevel() {
        return difficultyLevel;
    }
    public double getAttackSpeedMultiplier() {
        return attackSpeedMultiplier;
    }
    
    class HealthBar extends Actor {
        public HealthBar(GreenfootImage image) {
            setImage(image);
        }

        public void act() {
 
        }
    }
}