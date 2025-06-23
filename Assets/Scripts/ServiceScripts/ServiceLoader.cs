using UnityEngine;

public static class ServiceLoader
{

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        //Find the config scriptable holding all editable values
        var config = Resources.Load<ServiceConfig>("ServiceConfig");

        //If not found throw an exception
        if (config == null) throw new System.Exception("Service Config scriptalbe not found in Resources folder.");

        //Call the SetUpServices method to instantiate all services and set up required references and values
        config.SetUpServices();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void InitializeServices()
    {
        //Tell the service locator to initialize all services
        Services.InitializeServices();
    }
}
