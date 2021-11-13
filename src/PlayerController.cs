using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
    public GameManager gameManager;
    public PlayerEntity player;
    public GUIManager guiManager;
	
	void Update () 
    {
        if (player != null && !GC.PAUSED)
        {
            // get input (-1 to 0 to 1)
            float ad = Input.GetAxisRaw("Horizontal");
            float ws = Input.GetAxisRaw("Vertical");

            // get direction from input
            Vector3 leftRightMovement = player.transform.right * ad;
            Vector3 forwardBackwardMovement = player.transform.forward * ws;
            Vector3 dir = leftRightMovement + forwardBackwardMovement;
            dir.Normalize();

            // send movement to entity
            player.Move(dir);

            // send mouse movement
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            player.mouseLook(mouseX, mouseY);

            // shoot (LMB)
            if (Input.GetButton("Fire1"))
            {
                player.shoot();
            }

            // start crouch (CTRL)
            if (Input.GetButtonDown("Crouch"))
            {
                player.enableCrouch();
            }
            // stop crouch (CTRL)
            else if (Input.GetButtonUp("Crouch"))
            {
                player.disableCrouch();
            }

            // weapons hotbar
            if (Input.GetButtonUp("Pistol"))
            {
                player.setWeapon(WeaponManager.pistol);
            }
            if (Input.GetButtonUp("Shotgun"))
            {
                player.setWeapon(WeaponManager.shotgun);
            }
            if (Input.GetButtonUp("Rifle"))
            {
                player.setWeapon(WeaponManager.rifle);
            }

            // cycle weapon
            if (Input.GetButtonUp("Cycle Weapon"))
            {
                player.nextWeapon();
            }
            if (Input.GetButtonUp("Next Weapon"))
            {
                player.nextWeapon();
            }
            else if (Input.GetButtonUp("Previous Weapon"))
            {
                player.previousWeapon();
            }

            // game menu
            if (Input.GetButtonUp("GameMenu"))
            {
                gameManager.onOpenGameMenu();
            }
        }
        else if (GC.STATE == GC.GAMEMENU && Input.GetButtonUp("GameMenu"))
        {
            gameManager.onCloseGameMenu();
        }
        // menu movement
        if (GC.STATE != GC.INGAME)
        {
            if (Input.GetButtonUp("Submit") || Input.GetButtonUp("Submit Joy"))
            {
                guiManager.inputEnter();
            }
            if (Input.GetButtonDown("Menu Up") || Input.GetButtonDown("Menu Up Joy"))
            {
                guiManager.inputUp();
            }
            else if (Input.GetButtonDown("Menu Down") || Input.GetButtonDown("Menu Down Joy"))
            {
                guiManager.inputDown();
            }
            if (GC.STATE == GC.MAINMENU && Input.GetButtonUp("Anime"))
            {
                gameManager.onAnime();
            }
        }
	}
}