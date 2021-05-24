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
    public Button subirApuestaButton;
    public Button bajarApuestaButton;
    public Button apostarButton;
    public Text finalMessage;
    public Text probMessage;
    public Text probMessage2;
    public Text probMessage3;
    public Text valorApuestaMsg;

    public Text bancaEstado;
    
    private int tipoCarta = 4;
    private int nCartasTipo = 13;

    private bool apostando = false;
    

    public int[] values = new int[52];
    int cardIndex = 0;

    private int banca = 1000;
    private int valorApuesta = 10;
    
    private bool cartaOculta;
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
                if (i == 1)
                {
                    values[i] = 11;
                }
                else
                {
                    values[i] = pilaTipoCarta;
                }
                
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
            PushDealer();
            PushPlayer();
        }
        
        /*TODO:
        * Si alguno de los dos obtiene Blackjack, termina el juego y mostramos mensaje
        */
        
        playerPoints = player.GetComponent<CardHand>().getPoints();
        dealerPoints = dealer.GetComponent<CardHand>().getPoints();

        blackJack = ComprobarBlackJack(playerPoints, dealerPoints);
        
        if (!blackJack)
        { 
            ComprobarFinDePartida(playerPoints,dealerPoints);
        }
        else
        {
            PonerEstadoFinPartida();
        }

        ActualizarTextoBanca(banca);
        ActualizarTextoApuesta(valorApuesta);

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
        
        float probabilities;
        float PSA;
        float PSAUB;
        int casosFavorables;
        int CFSA;
        
        int valorDealerTotal = dealer.GetComponent<CardHand>().getPoints();
        int valorJugadorTotal = player.GetComponent<CardHand>().getPoints();
        int valorDealerSecreto = dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().value;

        if (dealer.GetComponent<CardHand>().cards.Count == 2 && player.GetComponent<CardHand>().cards.Count == 2)
        {
            cartaOculta = true;
        }
        else
        {
            cartaOculta = false;
        }
        
        if (cartaOculta)
        {
            int valorDealerSinSecreto = valorDealerTotal - valorDealerSecreto;
            casosFavorables = nCartasTipo - valorJugadorTotal + valorDealerSinSecreto;
            probabilities = casosFavorables / 13f;

            probabilities = redondeoProb(probabilities);

            probMessage.text = "P crupier más puntos "+Math.Round(probabilities * 100).ToString() + " %";
            //Debug.Log("Probabilidad de que el crupier tenga más puntos "+Math.Round(probabilities * 100).ToString() + " %");
        }
        
        casosFavorables = nCartasTipo - (21 - valorJugadorTotal);
        probabilities = casosFavorables / 13f;
        probabilities = redondeoProb(probabilities);
        probMessage2.text = "P jugador obtenga más de 21 si pide carta "+Math.Round(probabilities * 100).ToString() + " %";

        CFSA = nCartasTipo - (16 - valorJugadorTotal);
        PSA = CFSA / 13f;
        PSA = redondeoProb(PSA);
        PSAUB = PSA - probabilities;
        PSAUB = redondeoProb(PSAUB);
        probMessage3.text = "P jugador obtenga entre 17 y 21 si pide carta "+Math.Round(PSAUB*100).ToString() +" %";
        
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
        if (dealer.GetComponent<CardHand>().cards.Count == 2)
        {
          GirarPrimeraCartaDealer();  
        }
        
        //Repartimos carta al jugador
        
        PushPlayer();
        
        
        /*TODO:
         * Comprobamos si el jugador ya ha perdido y mostramos mensaje
         */     
        ComprobarFinDePartida(player.GetComponent<CardHand>().getPoints(),dealer.GetComponent<CardHand>().getPoints());

    }

    public void Stand()
    {
        /*TODO: 
         * Si estamos en la mano inicial, debemos voltear la primera carta del dealer.
         */
        if (dealer.GetComponent<CardHand>().cards.Count == 2)
        {
            GirarPrimeraCartaDealer();  
        }

        /*TODO:
         * Repartimos cartas al dealer si tiene 16 puntos o menos
         * El dealer se planta al obtener 17 puntos o más
         * Mostramos el mensaje del que ha ganado
         */
        if (dealer.GetComponent<CardHand>().getPoints() <= 16)
        {
            PushDealer();
        }

        if (dealer.GetComponent<CardHand>().getPoints() >= 17)
        {
            finalMessage.text = "El crupier se planta, el jugador gana";
            PonerEstadoFinPartida();
        }
         
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

    public void SubirApuesta()
    {
        if (valorApuesta+10 <= banca)
        {
            valorApuesta =valorApuesta+10;
            ActualizarTextoApuesta(valorApuesta);
        }
    }

    public void BajarApuesta()
    {
        if (valorApuesta-10 >= 0)
        {
            valorApuesta =valorApuesta-10;
            ActualizarTextoApuesta(valorApuesta);
        }
    }

    public void ActivarApuesta()
    {
        if (apostando)
        {
            apostando = false;
        }
        else
        {
            apostando = true;
        }
    }
    

    private bool ComprobarBlackJack(int playerPoints, int dealerPoints)
    {
        if (playerPoints == 21 )
        { 
            finalMessage.text = "BLACKJACK!! Fin de la partida, ha ganado el Jugador";
            GirarPrimeraCartaDealer();
            return true;
        }else if (dealerPoints == 21)
        {
            finalMessage.text = "BLACKJACK!! Fin de la partida, ha ganado el crupier";
            GirarPrimeraCartaDealer();
            return true;
        }else
        {
            return false;
        }
    }

    private void ComprobarFinDePartida(int playerPoints, int dealerPoints)
    {
        if (playerPoints == 21)
        {
            finalMessage.text = "Fin de la partida, ha ganado el jugador";
            PonerEstadoFinPartida();
        }else if (playerPoints > 21)
        {
            finalMessage.text = "Fin de la partida, gana el crupier";
            PonerEstadoFinPartida();
        }else if (dealerPoints > 21)
        {
            finalMessage.text = "Fin de la partida, gana el jugador";
            PonerEstadoFinPartida();
        }else if (dealerPoints == 21)
        {
            finalMessage.text = "Fin de la partida, gana el crupier ";
                PonerEstadoFinPartida();
        }
    }

    private void PonerEstadoFinPartida()
    {
        stickButton.interactable = false;
        hitButton.interactable = false;
    }

    private void GirarPrimeraCartaDealer()
    {
        dealer.GetComponent<CardHand>().cards[0].GetComponent<CardModel>().ToggleFace(true);
        
    }

    public void ComprobarCartas()
    {
        for (int i = 1; i <= tipoCarta; i++)
        {
            Debug.Log("VALOR CARTA "+i+": "+values[i]);
        }
    }

    private float redondeoProb(float prob)
    {
        if (prob > 1)
        {
            prob = 1;
        }else if (prob < 0)
        {
            prob = 0;
        }

        return prob;
    }

    private void ActualizarTextoBanca(int bancaEstadoNuevo)
    {
        bancaEstado.text = "Banca del jugador: " + bancaEstadoNuevo;
        Debug.Log("Banca del jugador: "+banca);
    }

    private void ActualizarTextoApuesta(int apuestaActualizada)
    {
        valorApuestaMsg.text = "Valor apuesta actual "+apuestaActualizada+"€";
        Debug.Log("Valor apuesta actual "+valorApuesta);
    }
    
}
