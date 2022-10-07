using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Statistics
{
    public int health;
    public int strength;
    public int magic;
    public int defense;
    public int resistance;
    public int proficiency;
    public int motivation;
    public int agility;
    public int movement;
     
    public Statistics (int hp = 0, int str = 0, int mag = 0, int def = 0, int res = 0, int prof = 0, int mot = 0, int agi = 0, int mov = 0)
    {
        //Constructor
        //When creating a Stat object instance we can define each stat if we need otherwise they default to 0
        health = hp;
        strength = str;
        magic = mag;
        defense = def;
        resistance = res;
        proficiency = prof;
        motivation = mot;
        agility = agi;
        movement = mov;
    }

    // Overload operride 

    //very cool thing we can do where we can define how operaters affect instances of the struct
    // ie: we can define adding two of the struct together and it will add all of the stats

    //note these are used for the WHOLE object so if we just need to compare say strength we can just use statistics.strength +-*/etc.

    //I imagine stuff like this will become useful when we implemenet skills
    //We can also add more operators or redefine how they work in the future. Maybe a BST operator could be useful etc. 


    //addition
    public static Statistics operator +(Statistics a, Statistics b) 
        => new Statistics
        (
            (a.health + b.health),
            (a.strength + b.strength),
            (a.magic + b.magic),
            (a.defense + b.defense),
            (a.resistance + b.resistance),
            (a.proficiency + b.proficiency),
            (a.motivation + b.motivation),
            (a.agility + b.agility),
            (a.movement + b.movement)               
        );
    
    //subtraction
    public static Statistics operator -(Statistics a, Statistics b) 
        => new Statistics
        (
            (a.health - b.health),
            (a.strength - b.strength),
            (a.magic - b.magic),
            (a.defense - b.defense),
            (a.resistance - b.resistance),
            (a.proficiency - b.proficiency),
            (a.motivation - b.motivation),
            (a.agility - b.agility),
            (a.movement - b.movement)               
        );

    //multiplication
    public static Statistics operator *(Statistics a, Statistics b) 
        => new Statistics
        (
            (a.health * b.health),
            (a.strength * b.strength),
            (a.magic * b.magic),
            (a.defense * b.defense),
            (a.resistance * b.resistance),
            (a.proficiency * b.proficiency),
            (a.motivation * b.motivation),
            (a.agility * b.agility),
            (a.movement * b.movement)             
        );

    //division
    public static Statistics operator /(Statistics a, Statistics b) 
        => new Statistics
        (
            (a.health / b.health),
            (a.strength / b.strength),
            (a.magic / b.magic),
            (a.defense / b.defense),
            (a.resistance / b.resistance),
            (a.proficiency / b.proficiency),
            (a.motivation / b.motivation),
            (a.agility / b.agility),
            (a.movement / b.movement)
        );

}

[System.Serializable]
public struct Growthrates
{
    public int healthGrowth;
    public int strengthGrowth;
    public int magicGrowth;
    public int defenseGrowth;
    public int resistanceGrowth;
    public int proficiencyGrowth;
    public int agilityGrowth;

    public Growthrates(int hp = 0, int str = 0, int mag = 0, int def = 0, int res = 0, int prof = 0, int agi = 0)
    {
        //Constructor
        healthGrowth = hp;
        strengthGrowth = str;
        magicGrowth = mag;
        defenseGrowth = def;
        resistanceGrowth = res;
        proficiencyGrowth = prof;
        agilityGrowth = agi;
    }
}

    
    
    


    

