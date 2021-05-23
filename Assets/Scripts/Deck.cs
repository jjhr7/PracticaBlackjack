using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Deck : MonoBehaviour
{
    public Sprite[] faces;
    public GameObject dealer;
    public GameObject player;
    public Button hitButton;
    public Button stickButton;
    public Button playAgainButton;
    public Text finalMessage;
    public Text probMessage;
    private int tipoCarta = 4;
    private int nCartasTipo = 13;

    public int[] values = new int[52];
    int cardIndex = 0;    
       
    private void Awake()
    {    
        InitCardValues();
    }

    private void Start()
    {
        ShuffleCards();
        StartGame();        
    }

    private void InitCardValues()
    {
        /*TODO:
         * Asignar un valor a cada una de las 52 cartas del atributo "values".
         * En principio, la posición de cada valor se deberá corresponder con la posición de faces. 
         * Por ejemplo, si en faces[1] hay un 2 de corazones, en values[1] debería haber un 2.
         */

        int pilaTipoCarta = 1;

        for (int i = 0; i < 52; i++)
        {
            if (pilaTipoCarta == 14)
            {
                pilaTipoCarta = 1;
            }
            
            if (pilaTipoCarta >= 10)
            {
                values[i] = 10;
                
            }
            else
            {
                values[i] = pilaTipoCarta;
            }
            
            pilaTipoCarta++;
        }
        //ComprobarCartas();
    }

    private void ShuffleCards()
    {
        /*TODO:
         * Barajar las cartas aleatoriamente.
         * El método Random.Range(0,n), devuelve un valor entre 0 y n-1
         * Si lo necesitas, puedes definir nuevos arrays.
         */

        int posFutura = -1;
        int auxValue = -1;
        Sprite auxFace = null;
        
        
        for (int i =0; i < values.Length;i++)
        {
            posFutura = Random.Range(0, 52);
            auxValue = values[posFutura];
            auxFace = faces[posFutura];
            
            values[posFutura] = values[i];
            faces[posFutura] = faces[i];
            
            values[i] = auxValue;
            faces[i] = auxFace;
        }
        
        //ComprobarCartas();
    }

    void StartGame()
    {
        int playerPoints = 0;
        int dealerPoints = 0;
        bool blackJack = false;
        
        for (int i = 0; i < 2; i++)
        {
            PushPlayer();
            PushDealer();
        }
        
        /*TODO:
        * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
        */
        
        playerPoints = player.GetComponent<CardHand>().getPoints();
        dealerPoints = dealer.GetComponent<CardHand>().getPoints();

        blackJack = comprobarBlackJack(playerPoints, dealerPoints);
        
        if (!blackJack)
        { 
            comprobarFinDePartida(playerPoints,dealerPoints);
        }
        else
        {
            ponerEstadoFinPartida();
        }
        
        //Debug.Log("Putuación del Jugador: "+playerPoints+" - Puntuación del Crupier: "+dealerPoints);
    }

    private void CalculateProbabilities()
    {
        /*TODO:
         * Calcular las probabilidades de:
         * - Teniendo la carta oculta, probabilidad de que el dealer tenga más puntuación que el jugador
         * - Probabilidad de que el jugador obtenga entre un 17 y un 21 si pide una carta
         * - Probabilidad de que el jugador obtenga más de 21 si pide una carta          
         */
    }

    void PushDealer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        dealer.GetComponent<CardHand>().Push(faces[cardIndex],values[cardIndex]);
        cardIndex++;        
    }

    void PushPlayer()
    {
        /*TODO:
         * Dependiendo de cómo se implemente ShuffleCards, es posible que haya que cambiar el índice.
         */
        player.GetComponent<CardHand>().Push(faces[cardIndex], values[cardIndex]/*,cardCopy*/);
        cardIndex++;
        CalculateProbabilities();
    }       

    public void Hit()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        
        //Repartimos carta al jugador
        PushPlayer();

        /*TODO:
         * Comprobamos si el jugador ya ha perdido y mostramos mensaje
         */      

    }

    public void Stand()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */

        /*TODO:
         * Repartimos cartas al dealer si tiene 16 puntos o menos
         * El dealer se planta al obtener 17 puntos o más
         * Mostramos el mensaje del que ha ganado
         */                
         
    }

    public void PlayAgain()
    {
        hitButton.interactable = true;
        stickButton.interactable = true;
        finalMessage.text = "";
        player.GetComponent<CardHand>().Clear();
        dealer.GetComponent<CardHand>().Clear();          
        cardIndex = 0;
        ShuffleCards();
        StartGame();
    }

    public bool comprobarBlackJack(int playerPoints, int dealerPoints)
    {
        if (playerPoints == 21 )
        { 
            finalMessage.text = "BLACKJACK!! Fin de la partida, ha ganado el Jugador";
            return true;
        }else if (dealerPoints == 21)
        {
            finalMessage.text = "BLACKJACK!! Fin de la partida, ha ganado el crupier";
            return true;
        }else
        {
            return false;
        }
    }

    public void comprobarFinDePartida(int playerPoints, int dealerPoints)
    {
        if (playerPoints == 21)
        {
            finalMessage.text = "Fin de la partida, ha ganado el jugador";
            ponerEstadoFinPartida();
        }else if (playerPoints > 21)
        {
            finalMessage.text = "Fin de la partida, gana el crupier porque la suma de los valores de las cartas del jugador es mayor a 21";
            ponerEstadoFinPartida();
        }else if (dealerPoints > 21)
        {
            finalMessage.text = "Fin de la partida, gana el jugador porque la suma de los valores de las cartas del crupier es mayor a 21";
            ponerEstadoFinPartida();
        }else if (dealerPoints == 21)
        {
            finalMessage.text = "Fin de la partida, gana el crupier ";
            ponerEstadoFinPartida();
        }
    }

    public void ponerEstadoFinPartida()
    {
        stickButton.interactable = false;
        hitButton.interactable = false;
    }

   /* public void ComprobarCartas()
    {
        for (int i = 1; i <= tipoCarta; i++)
        {
            for (int j = nCartasTipo; j >=1; j--)
            {
                Debug.Log("VALOR CARTA: "+values[j]);
            }
        }
    }*/
    
}
