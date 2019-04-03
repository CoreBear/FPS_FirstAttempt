# FPS_FirstAttempt
My first attempt!

As the title states, this is the first attempt at a FPS game. What I learned from this, was an understanding of controller input in Unity. Currently, the game is set up for the PS4 controller. However, if you go into the following functions in the "ControllerInput" script, you will be able to swap over to an Xbox 360 controller. Uncomment 360 and comment Ps4 and you'll be good to go!

public void RunControllerGame()
{
    // Which controller to run
    //RunControllerGame360();
    RunControllerGamePS4();
}
public void RunControllerPaused()
{
    // Which controller to run
    //RunControllerPaused360();
    RunControllerPausedPS4();
}
