using Microsoft.SemanticKernel;
using System.ComponentModel;

public class LightsPlugin
{
    private readonly List<LightModel> _lights = new()
    {
        new LightModel { Id = 1, Name = "Sala", IsOn = false },
        new LightModel { Id = 2, Name = "Cozinha", IsOn = false },
        new LightModel { Id = 3, Name = "Quarto", IsOn = false }
    };

    [KernelFunction]
    [Description("Obtém a lista de luzes e seus estados")]
    public List<LightModel> GetLights()
    {
        return _lights;
    }

    [KernelFunction]
    [Description("Liga ou desliga uma luz")]
    public string SetLightState(
        [Description("Id da luz")] int id,
        [Description("Estado desejado")] bool isOn)
    {
        var light = _lights.FirstOrDefault(l => l.Id == id);

        if (light == null)
            return "Luz não encontrada";

        light.IsOn = isOn;

        return $"Luz {light.Name} agora está {(isOn ? "ligada" : "desligada")}";
    }
}