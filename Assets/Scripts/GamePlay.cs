using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using TMPro;
using TMPro.Examples;
using UnityEngine.UIElements;

public class GamePlay : MonoBehaviour
{

    [SerializeField]
    GameObject restartButton;
    [SerializeField]
    GameObject startMenuButton;

    private bool gameOver;
    
    [SerializeField]
    GameObject tile;
    [SerializeField]
    GameObject boostTile;
    [SerializeField]
    GameObject emptyTile; // This tile will be instantiated but disabled
    [SerializeField]
    GameObject suppliesTile;
    [SerializeField]
    GameObject stickyTile;
    [SerializeField]
    GameObject burningTile;
    [SerializeField]
    GameObject obstacleTile;

    [SerializeField]
    TMP_Text gameOverText;
    [SerializeField]
    TMP_Text supplementaryText;

    [SerializeField]
    TMP_Text speedText;

    private float score;
    private float scoreRate = 1.0f;
    [SerializeField]
    TMP_Text score_text;

    private float fuel;
    private float fuelRate = 1.0f;
    [SerializeField]
    TMP_Text fuel_text;

    [SerializeField]
    float jumpVelocity;

    [SerializeField]
    float speed;

    private Rigidbody rb;

    private bool normalSpeed;

    private bool isPaused = false;

    [SerializeField] 
    AudioSource speedSound;
    [SerializeField]
    AudioSource stickySound;
    [SerializeField]
    AudioSource suppliesSound;
    [SerializeField]
    AudioSource burningSound;
    [SerializeField]
    AudioSource obstacleSound;
    [SerializeField]
    AudioSource nopeSound;




    // Constants for tile positioning
    private float[] xPositions = { -2.75f, 0f, 2.75f }; // Possible X positions
    private float yPosition = 0f; // Y position stays constant
    private float zIncrement = 7.5f; // Z increment per row of tiles

    private List<GameObject> tiles = new List<GameObject>(); // List to keep track of generated tiles
    private int totalRows = 100; // Total number of rows to generate
    private float lastZPosition = 0f; // Keep track of the last Z position where tiles were generated

    void GenerateTiles()
    {
        // Create an array of the tile prefabs, including the empty tile
        GameObject[] tileTypes = { tile, tile, tile, tile, tile, tile, boostTile, suppliesTile, stickyTile, burningTile, emptyTile, obstacleTile };


        for (int zIndex = 0; zIndex < totalRows; zIndex++)
        {
            float zPosition = lastZPosition + (zIndex * zIncrement);

            // Randomly select a tile type for the first column
            GameObject randomTileType1 = tileTypes[Random.Range(0, tileTypes.Length)];
            GameObject newTile1 = Instantiate(randomTileType1, new Vector3(xPositions[0], yPosition, zPosition), Quaternion.identity);
            if (randomTileType1 == emptyTile)
            {
                newTile1.SetActive(false); // If it is an empty tile, disable it
            }
            tiles.Add(newTile1);

            // Randomly select a tile type for the second column
            GameObject randomTileType2 = tileTypes[Random.Range(0, tileTypes.Length)];
            GameObject newTile2 = Instantiate(randomTileType2, new Vector3(xPositions[1], yPosition, zPosition), Quaternion.identity);
            if (randomTileType2 == emptyTile)
            {
                newTile2.SetActive(false); // If it is an empty tile, disable it
            }
            tiles.Add(newTile2);

            // Randomly select a tile type for the third column
            GameObject randomTileType3 = tileTypes[Random.Range(0, tileTypes.Length)];
            GameObject newTile3 = Instantiate(randomTileType3, new Vector3(xPositions[2], yPosition, zPosition), Quaternion.identity);
            if (randomTileType3 == emptyTile)
            {
                newTile3.SetActive(false); // If it is an empty tile, disable it
            }
            tiles.Add(newTile3);

            // Ensure at least one normal tile, supplies tile, or boost tile in this row
            if ((randomTileType1 == emptyTile || randomTileType1 == burningTile || randomTileType1 == stickyTile || randomTileType1 == obstacleTile) &&
                (randomTileType2 == emptyTile || randomTileType2 == burningTile || randomTileType2 == stickyTile || randomTileType1 == obstacleTile) &&
                (randomTileType3 == emptyTile || randomTileType3 == burningTile || randomTileType3 == stickyTile || randomTileType1 == obstacleTile))
            {
                // Randomly select which column to replace
                int columnToReplace = Random.Range(0, 3);
                // Randomly select a "kind" tile type.
                GameObject replacementTileType = tileTypes[Random.Range(0, 3)]; //0 to 3 because the tile type array has the "kind" types at the start


                // Remove the old tile and replace it with the new one
                int indexToReplace = tiles.Count - 3 + columnToReplace; // Get the correct index
                Destroy(tiles[indexToReplace]); // Destroy the old tile
                tiles[indexToReplace] = Instantiate(replacementTileType, new Vector3(xPositions[columnToReplace], yPosition, zPosition), Quaternion.identity); // Replace with the new tile
            }
        }

        // Update the lastZPosition to the last generated position
        lastZPosition += totalRows * zIncrement;
        
    }




    void RemoveOldTiles()
    {
        // Calculate the threshold Z position beyond which tiles should be removed
        float removeThreshold = transform.position.z - (totalRows / 4) * zIncrement;

        // Loop through the tiles list in reverse order to avoid index issues when removing
        for (int i = tiles.Count - 1; i >= 0; i--)
        {
            GameObject tile = tiles[i];
            // Check if the tile's Z position is below the removal threshold
            if (tile.transform.position.z < removeThreshold)
            {
                Destroy(tile); // Destroy the tile
                tiles.RemoveAt(i); // Remove from the list
            }
        }

        // After removing old tiles, generate new tiles if needed
        if (transform.position.z > lastZPosition - (totalRows / 1) * zIncrement)
        {
            GenerateTiles(); // Generate new tiles in the forward direction
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("boost") && normalSpeed == true)
        {
            speed = speed * 2;
            normalSpeed = false;
            if (!speedSound.isPlaying)
            {
                speedSound.Play();
            }
            
        }
        if(collision.gameObject.CompareTag("sticky") && normalSpeed == false)
        {
            speed = speed / 2;
            normalSpeed = true;
            if (!stickySound.isPlaying)
            {
                stickySound.Play();
            }
        }
        if (collision.gameObject.CompareTag("burning"))
        {
            fuelRate = 10.0f;
            if (!burningSound.isPlaying)
            {
                burningSound.Play();
            }
        }
        if (collision.gameObject.CompareTag("supplies"))
        {
            fuel = 50;
            if (!suppliesSound.isPlaying)
            {
                suppliesSound.Play();
            }

        }
        if (collision.gameObject.CompareTag("obstacle"))
        {
            gameOver = true;
            supplementaryText.SetText("You've hit an obstacle");
            if (!obstacleSound.isPlaying)
            {
                obstacleSound.Play();
            }
        }
    }

    private void OnCollisionExit(Collision collision) 
    {
        if (collision.gameObject.CompareTag("burning"))
        {
            fuelRate = 1.0f;
        }
    }

    void Start()
    {
        Debug.Log("started");
        isPaused = false;

        normalSpeed = true;

        fuel = 50;

        gameOverText.SetText("");
        supplementaryText.SetText("");

        GameObject startingTile = Instantiate(tile, new Vector3(xPositions[1], yPosition, -zIncrement), Quaternion.identity);
        tiles.Add(startingTile);

        GenerateTiles(); // Initial tile generation

        rb = this.gameObject.GetComponent<Rigidbody>();

        // Move the player forward with constant velocity
        rb.velocity = new Vector3(0, 0, speed);

        // Initially hide the restart and the start menu button
        restartButton.SetActive(false);
        startMenuButton.SetActive(false);

    }


    private void TogglePause()
    {
        isPaused = !isPaused; // Toggle the pause state
        restartButton.SetActive(isPaused);
        startMenuButton.SetActive(isPaused);

        if (isPaused)
        {
            // Optionally show a pause menu or a pause message
            Time.timeScale = 0; // Freeze time in the game
            gameOverText.SetText("Game Paused"); // Display pause message
        }
        else
        {
            Time.timeScale = 1; // Resume time in the game
            gameOverText.SetText(""); // Clear pause message
        }
    }


    private void LateUpdate()
    {
        if (gameOver == true)
        {
            speed = 0;
            jumpVelocity = 0;
            gameOverText.SetText("Game Over");
            // Show the restart button when the game is over
            restartButton.SetActive(true);
            startMenuButton.SetActive(true);
        }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause(); // Call the pause toggle method
        }

        if (isPaused)
        {
            return; // Skip the rest of the Update method if paused
        }

        if (normalSpeed == true)
        {
            speedText.SetText("Normal Speed");
        }
        else
        {
            speedText.SetText("High Speed");
        }

        if (rb.position.y < -0.5)
        {
            gameOver = true;
            supplementaryText.SetText("You've fallen into the abyss");
        }

        if (fuel <= 0)
        {
            gameOver = true;
            fuel = 0;
            supplementaryText.SetText("You've ran out of fuel");
        }

        if( gameOver != true)
        {
            score += scoreRate * Time.deltaTime;
            fuel -= fuelRate * Time.deltaTime;
        }

        score_text.SetText("Score: " + Mathf.FloorToInt(score));

        

        fuel_text.SetText("Fuel: " + Mathf.FloorToInt(fuel));

        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, speed);

        if (Input.GetKeyDown(KeyCode.A) && transform.position.x > -2.75f && !gameOver)
        {
            transform.Translate(-2.75f, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.A) && transform.position.x == -2.75f && !gameOver)
        {
            if (!nopeSound.isPlaying)
            {
                nopeSound.Play();
            }
        }
        if (Input.GetKeyDown(KeyCode.D) && transform.position.x < 2.75f && !gameOver)
        {
            transform.Translate(2.75f, 0, 0);
        }
        if (Input.GetKeyDown(KeyCode.D) && transform.position.x == 2.75f && !gameOver)
        {
            if (!nopeSound.isPlaying)
            {
                nopeSound.Play();
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) && transform.position.y < 1.0f && transform.position.y > -0.3f)
        {
            rb.velocity = new Vector3(0, jumpVelocity, 0);
        }
        
            // Remove old tiles and generate new ones if necessary
            RemoveOldTiles();
    }
}
