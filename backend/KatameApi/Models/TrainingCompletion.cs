namespace KatameApi.Models;

// Un registro por cada día que el usuario marcó "ya entrené hoy". Es lo único
// que hacía falta para poder calcular la racha de entrenamiento -- Training
// solo guardaba el plan (qué tocaba cada día), nunca si en verdad se hizo.
public class TrainingCompletion : IUserOwned
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }
}
