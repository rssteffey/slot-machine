# Slot Machine - Piggy Parade

In 2019, we threw a casino-themed party for Alexandria's birthday. As an extra activity to the card games, 
I designed this slot machine as a replica of one we became hooked on during a trip to Shreveport. \
I'm putting this out there as a resource since it's just been sitting on a hard drive in the closet, slowly deteriorating.

- It is not balanced.
- It is not original (although given what little I know about copyrighting game mechanics, you could probably change the logo/title and be totally fine).
- And most of all - It's not going to work entirely out-of-the-box


Click on the video below for a quick overview of how it's intended to be used: 

[![IMAGE ALT TEXT HERE](https://img.youtube.com/vi/qLkZNvvKdpQ/0.jpg)](https://www.youtube.com/watch?v=qLkZNvvKdpQ)

## Setup

Not entirely taking the time to refresh my memory on all the steps, but if you're even 20% familiar with Unity it shouldn't be too hard to figure things out

### Input
 Since this was designed to be used in a standalone machine, the inputs are bound through Unity's input system. Rebind those however you need, but they worked great with a standard arcade USB encoder

### Fund Tracking
The fund management for the night was done through a very simple API running on a separate machine. \
You can self-host this project, found [here on Gitlab](https://gitlab.com/logiraffe/casino-api) (It's a bastardized version of the [Wordabeasts](http://www.logiraffe.com/wordabeasts) API, so it got the Gitlab host) \
Just be sure to swap the url at the top of UserManager.cs

```
const string apiHost = "your-api-host.com:4800"; //Swap for your API host
```

As seen in the video above, users log into the machine simply by showing a QR code.  
This is obviously not secure - but it's fantastic for a party where guests can share ID badges as needed.
If you don't need this piece, ripping it out/replacing it is on you to figure out.

### Balance
One of the most important caveats before you, reader, run off and build The Bellagio 2 on the framework presented here: \
On this machine, the house WILL lose.

People like winning. People at a party doubly so. \
Rather than bother with any earnings tables or payout math in the week of designing this before the party, I simply built the reels with the same logo patterns as the replicated machine.

The real machine has exact control over where the reels stop, and adjusts the payouts manually. \
This machine stops the reels randomly (theoretically like early physical machines would), so the only balancing is done by tweaking the frequency and spacing of symbols on each reel.

![image](https://user-images.githubusercontent.com/4682038/182770826-f03b5be1-040d-4e43-837a-73c6a7c8c8a2.png)

## Misc

- I know what 2019 Me was like, so my guess is there are random input checks all over the code without DEBUG flags and not grouped in one handy debug file.  You probably wanna kill all those so users can't trigger test behavior
- I'm sure there are 20 other ways this code is broken; if you're really stuck, open an issue here and I'll try to take a look and see if I remember anything



<img src='https://img.shields.io/github/license/rssteffey/slot-machine' />
