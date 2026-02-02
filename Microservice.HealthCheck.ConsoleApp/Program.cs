while (true)
{
    HttpClient httpClient = new();
    httpClient.BaseAddress = new Uri("http://localhost:5002");
    try
    {
        var message = await httpClient.GetAsync("health");
        var res = await message.Content.ReadAsStringAsync();
        if (res != "Healthy")
        {
            //Do something
        }
    }
    catch (Exception)
    {
        //Do something        
    }

    await Task.Delay(1000);
}