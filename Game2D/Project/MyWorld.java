import greenfoot.*;
import java.util.List;
import java.util.Random;

public class MyWorld extends World {
    private Hero hero;
    double coef=1;
    private int enemySpawnRate = (int)(600/coef);
    private int enemySpawnCounter = 0;
    private int maxEnemies = (int)(8*coef);
    private int elapsedTime = 0;
    private int killCount = 0;
    private int score = 0;
    private Diff currDiff;
    

    private Label ammoLabel;
    private HealthBar healthBar;
    private Label timerLabel;
    private Label scoreLabel;
    private Label endGameLabel;

    private int maxZombies = (int)(25*coef);
    private int zombieSpawnRate = (int)(900/coef);

    private boolean gameEnded = false;

    private boolean portalSpawned = false;
    private boolean portal2Spawned = false;
    private boolean portal3Spawned = false;
    
    
    public MyWorld(Diff difficulty) {
        super(1200, 846, 1);
        this.currDiff = difficulty;
        prepare();
    }
    
    private void prepare() {
    
        switch (currDiff) {
            case EASY:
                coef=0.5;
                break;
            case MEDIUM:
                coef=1;
                break;
            case HARD:
                coef=1.5;
                break;
            case HARDCORE:
                coef=2;
                break;
        }
        
        if (hero == null && this instanceof MAP) {
        hero = new Hero(200, 190, 0);
        addObject(hero, 200, 190);
        }


        ammoLabel = new Label("Боезапас: 10", 26);
        addObject(ammoLabel, 100, 50);
        
        healthBar = new HealthBar(3000);
        addObject(healthBar, 100, 30);

        timerLabel = new Label("Время: 00:00", 26);
        addObject(timerLabel, 100, 80);

        scoreLabel = new Label("Очки: 0", 26);
        addObject(scoreLabel, 100, 110);

        endGameLabel = new Label("", 100);
        addObject(endGameLabel, getWidth() / 2, getHeight() / 2);
        
        

        
}

    public void act() {
        
        setPaintOrder(Label.class, HealthBar.class, Wall_vertical.class, Wall_gorizont.class, Cube.class , Bullet.class, Enemy_beast.class, Hero.class);
        
        enemySpawnCounter++;
        
        if (!(this instanceof MAP5)){
            for (int i = 0; i < maxEnemies; i++) {
            if (enemySpawnCounter>=enemySpawnRate) {
                spawnEnemyRandomly();
                enemySpawnCounter=0;
            }
            }
        }
        
        if (gameEnded) return;

        if (!hero.isAlive()) {
            endGame(false);
            return;
        }

        checkKeys();
        elapsedTime++;

        updateTimerDisplay();

        
        if(!(this instanceof MAP5)){
            enemySpawnCounter++;
            if (enemySpawnCounter >= enemySpawnRate) {
                enemySpawnCounter = 0;
                spawnEnemyRandomly();
            }
        }
        

        if (elapsedTime % zombieSpawnRate == 0 && (this instanceof MAP)) {
            if (getObjects(zombi.class).size() < maxZombies) {
                spawnZombie();
            }
        }

        

        

        if (this instanceof MAP && score >= (int)(400*coef) ) {
            if (!portalSpawned) {
            spawnPortal();
            portalSpawned = true;
        } else if (hero.checkPortalCollision()) {
        //Greenfoot.setWorld(new MAP4(currDiff,hero));
        Greenfoot.setWorld(new MAP3(currDiff,hero));
        }
        }

        if (this instanceof MAP3 && score >= (int)(600*coef)) {
            if (!portal2Spawned) {
            spawnPortal2();
            portal2Spawned = true;
        } else if (hero.checkPortal2Collision()) {
        Greenfoot.setWorld(new MAP4(currDiff,hero));
        }
        }
        
        if (this instanceof MAP4 && score >= (int)(800*coef)) {
            if (!portal3Spawned) {
            spawnPortal3();
            portal3Spawned = true;
        } else if (hero.checkPortal3Collision()) {
        Greenfoot.setWorld(new MAP5(currDiff,hero));
        }
        }
        hero.reload();
    }
    
    public void setHero(Hero h) {
        this.hero = h;
        this.hero.health=hero.getMaxHealth();
    }

    private void checkKeys() {
        int speed = 3;

        if (Greenfoot.isKeyDown("w")) hero.move(0, -speed);
        if (Greenfoot.isKeyDown("s")) hero.move(0, speed);
        if (Greenfoot.isKeyDown("a")) hero.move(-speed, 0);
        if (Greenfoot.isKeyDown("d")) hero.move(speed, 0);
        if (Greenfoot.mousePressed(null)) hero.shoot();
    }

    private void spawnEnemyRandomly() {
        Random rand = new Random();
        int x, y;
        do {
            x = rand.nextInt(getWidth());
            y = rand.nextInt(getHeight());
        } while (!isValidSpawnLocation(x, y));

        if (this instanceof MAP && getObjects(Enemy_beast.class).size() < maxEnemies) addObject(new Enemy_beast(x, y, Greenfoot.getRandomNumber(4),coef), x, y);
        if (this instanceof MAP4 && getObjects(RocketEnemy.class).size() < maxEnemies) addObject(new RocketEnemy(hero, coef), x, y);
        
        if (this instanceof MAP3 && getObjects(CosmoEnemy.class).size() < maxEnemies) addObject(new CosmoEnemy(hero, coef), x, y);
        if (this instanceof MAP3 && getObjects(ShotgunEnemy.class).size() < maxEnemies) addObject(new ShotgunEnemy(x, y, Greenfoot.getRandomNumber(4),coef,hero), x, y);
    }

    private boolean isValidSpawnLocation(int x, int y) {
    // Проверка самой точки
    List<Actor> objectsAt = getObjectsAt(x, y, Actor.class);
    for (Actor obj : objectsAt) {
        if (obj instanceof Wall_gorizont || obj instanceof Wall_vertical || obj instanceof Cube || obj instanceof Hero) {
            return false;
        }
    }

    // Проверка соседних клеток
    int[][] offsets = {
        { 0,  10}, { 10,  0}, { 0, -10}, {-10,  0}
    };

    for (int[] offset : offsets) {
        int nx = x + offset[0];
        int ny = y + offset[1];

        if (!getObjectsAt(nx, ny, Wall_gorizont.class).isEmpty()) return false;
        if (!getObjectsAt(nx, ny, Wall_vertical.class).isEmpty()) return false;
        if (!getObjectsAt(nx, ny, Cube.class).isEmpty()) return false;
    }

    return true;
}



    public void incrementKillCount() {
        killCount++;
        score += 40;
        updateScore();
    }

    private void updateScore() {
        scoreLabel.setValue("Очки: " + score);
    }

    public void updateAmmoDisplay(int ammo) {
        ammoLabel.setValue("Боезапас: " + ammo);
    }
    
    public HealthBar getHealthBar() {
        return healthBar;
    }

    

    private void spawnZombie() {
        Random rand = new Random();
        int x, y;
        do {
            x = rand.nextInt(getWidth());
            y = rand.nextInt(getHeight());
        } while (!isValidSpawnLocation(x, y));

        addObject(new zombi(hero,coef), x, y);
    }

    private void updateTimerDisplay() {
        int secondsLeft = elapsedTime / 60;
        int minutes = secondsLeft / 60;
        int seconds = secondsLeft % 60;
        String formatted = String.format("Время: %02d:%02d", minutes, seconds);
        timerLabel.setValue(formatted);
    }

    private void endGame(boolean win) {
        gameEnded = true;
        endGameLabel.setValue(win ? "YOU WIN" : "YOU LOSEE");
    }

    private void spawnPortal() {
        int x = getWidth() - 150; 
        int y = getHeight() / 2; 
        addObject(new Portal(), x, y);
    }
    
    private void spawnPortal2() {
        int x = getWidth() / 2; 
        int y = getHeight() - 200; 
        addObject(new Portal2(), x, y);
    }
    
    private void spawnPortal3() {
        int x = getWidth() / 2+10; 
        int y = getHeight() - 300; 
        addObject(new Portal3(), x, y);
    }
}
