using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Button dealBtn;
    public Button hitBtn;
    public Button standBtn;
    public Button betBtn;

    int standClicks = 0;

    public Player player;
    public Player dealer;

    public Text scoreText;
    public Text dealerScoreText;
    public Text betsText;
    public Text cashText;
    public Text mainText;
    public Text standBtnText;

    public GameObject hideCard;
    
    // how much is bet
    int pot = 0;

    void Start()
    {
        hitBtn.gameObject.SetActive(false);
        standBtn.gameObject.SetActive(false);
        mainText.text = "Press DEAL to start!";
        cashText.text = "$" + player.GetMoney().ToString();
        scoreText.text = "Hand: 0";
        betsText.text = "Bets: $0";
        dealerScoreText.gameObject.SetActive(false);

        dealBtn.onClick.AddListener(() => DealClicked());
        hitBtn.onClick.AddListener(() => HitClicked());
        standBtn.onClick.AddListener(() => StandClicked());
        betBtn.onClick.AddListener(() => BetClicked());
    }

    private void DealClicked()
    {
        if (pot <= 0)
        {
            mainText.text = "Please bet money first!";
            return;
        }
        player.ResetHand();
        dealer.ResetHand();
        // Hide dealer hand score at the start of the game
        mainText.gameObject.SetActive(false);
        dealerScoreText.gameObject.SetActive(false);
        GameObject.Find("Deck").GetComponent<Deck>().Shuffle();
        player.StartHand();
        dealer.StartHand();

        scoreText.text = "Hand: " + player.handValue.ToString();
        dealerScoreText.text = "Hand: " + dealer.handValue.ToString();

        hideCard.GetComponent<Renderer>().enabled = true;

        dealBtn.gameObject.SetActive(false);
        hitBtn.gameObject.SetActive(true);
        standBtn.gameObject.SetActive(true);
        standBtnText.text = "Stand";

        //pot = 40;
        betsText.text = "Bets: $" + pot.ToString();
        //player.AdjustMoney(-20);
        cashText.text = "$" + player.GetMoney().ToString();
    }

    private void HitClicked()
    {

        if (player.cardIndex <= 10)
        {
            player.GetCard();
            scoreText.text = "Hand: " + player.handValue.ToString();
            if (player.handValue > 20) RoundOver();
        }
    }
    private void StandClicked()
    {
        standClicks++;
        if (standClicks > 1) RoundOver();
        HitDealer();
        standBtnText.text = "Call";
    }

    private void HitDealer()
    {
        while (dealer.handValue < 16 && dealer.cardIndex < 10)
        {
            dealer.GetCard();
            //dealerScore
            dealerScoreText.text = "Hand: " + dealer.handValue.ToString();
            if (dealer.handValue > 20) RoundOver();
        }
    }

    void RoundOver()
    {
        bool playerBust = player.handValue > 21;
        bool dealerBust = dealer.handValue > 21;
        bool player21 = player.handValue == 21;
        bool dealer21 = dealer.handValue == 21;

        if (standClicks < 2 && !playerBust && !dealerBust && !player21 && !dealer21) return;
        bool roundOver = true;
        if (playerBust && dealerBust)
        {
            mainText.text = "All Bust: Bets returned";
            player.AdjustMoney(pot / 2);
        }

        else if (playerBust || !dealerBust && dealer.handValue > player.handValue)
        {
            mainText.text = "Dealer wins!";
        }

        else if (dealerBust || player.handValue > dealer.handValue)
        {
            mainText.text = "You win!";
            player.AdjustMoney(pot);
        }

        else if (player.handValue == dealer.handValue)
        {
            mainText.text = "Push: Bets returned";
            player.AdjustMoney(pot / 2);
        }
        else
        {
            roundOver = false;
        }

        if (roundOver)
        {
            hitBtn.gameObject.SetActive(false);
            standBtn.gameObject.SetActive(false);
            dealBtn.gameObject.SetActive(true);

            mainText.gameObject.SetActive(true);
            dealerScoreText.gameObject.SetActive(true);
            hideCard.GetComponent<Renderer>().enabled = false;
            cashText.text = "$" + player.GetMoney().ToString();
            standClicks = 0;
            pot = 0;
            betsText.text = "Bets: $" + pot.ToString();
        }
    }

    private void BetClicked()
    {
        if (player.GetMoney() <= 0)
        {
            mainText.text = "Sorry! You have no money!";
            return;
        }
        Text newBet = betBtn.GetComponentInChildren(typeof(Text)) as Text;
        int intBet = int.Parse(newBet.text.ToString().Remove(0, 1));
        player.AdjustMoney(-intBet);
        cashText.text = "$" + player.GetMoney().ToString();
        pot += (intBet * 2);
        betsText.text = "Bets: $" + pot.ToString();
    }
}
