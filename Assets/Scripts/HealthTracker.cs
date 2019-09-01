using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthTracker : MonoBehaviour
{
    public PlayerController myPlayer;
    private RoundManager rm;
    public int id;
	public int hp;
	public GameObject healthBar;
	public Animator healthAnim;
	public Animator flashAnim;
	private int maxHP = 6;

	// Start is called before the first frame update
	void Start()
    {
        hp = maxHP;
        healthBar.transform.localScale = new Vector3(1, 1, 1);
        id = myPlayer.playerID;
        rm = GameObject.FindGameObjectWithTag("Manager").GetComponent<RoundManager>();
    }

    public void TakeDamage(int taken)
    {
        hp -= taken;
        healthAnim.Play("HPbump");
        if(hp == 0)
        {
            Die();
        }
        else if (hp > 0)
        {
            flashAnim.Play("redFlash");
            healthBar.transform.localScale = new Vector3((float)hp / maxHP, 1, 1);
        }    
    }

    public void Die()
    {
        flashAnim.Play("redDead");
        Debug.Log("Player " + id.ToString() + " slain!");
        rm.updateScore(id);
    }

    public void Resurrect()
    {
        flashAnim.Play("redIdle");
        hp = maxHP;
        healthBar.transform.localScale = new Vector3(1, 1, 1);
    }
}