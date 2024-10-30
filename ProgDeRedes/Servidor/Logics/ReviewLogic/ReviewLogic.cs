using System.Text;
using Communication;
using Servidor.Collections;
using Servidor.Exceptions;
using Servidor.Logics.GameLogic;
using Servidor.Logics.UserLogic;

namespace Servidor.Logics.ReviewLogic;

static class ReviewLogic
{
    public static void HandleQualifyGame(NetworkDataHelper networkDataHelper, ref User currentUser)
    {
        try
        {

            byte[] lengthBytes = networkDataHelper.Receive(4);
            int dataLength = BitConverter.ToInt32(lengthBytes, 0);

            byte[] data = networkDataHelper.Receive(dataLength);
            string message = Encoding.UTF8.GetString(data);
            string[] parts = message.Split('#');

            string title = parts[0];
            string score = parts[1];
            string text = parts[2];


            Review review = new Review
            {
                User = currentUser,
                Score = int.Parse(score),
                Text = text
            };
            
            GameCollection.Instance.AddReview(title, currentUser, review);
            Program.SendResponse(networkDataHelper, "1#Reseña enviada.");
            Console.WriteLine($"Reseña de {title}" + " enviada.");
        }
        catch (ServerException e)
        {
            Program.SendResponse(networkDataHelper, e.Message);
        }

    }
    
    public static void HandleGetAllReviews(NetworkDataHelper networkDataHelper)
    {
        byte[] lengthBytes = networkDataHelper.Receive(4);
        int dataLength = BitConverter.ToInt32(lengthBytes, 0);

        byte[] data = networkDataHelper.Receive(dataLength);
        string message = Encoding.UTF8.GetString(data);

        List<Review> reviews = GameCollection.Instance.GetReviews(message);
        
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Reseñas de {message}");

        if (reviews.Count > 0)
        {
            foreach (var rev in reviews)
            {
                sb.AppendLine(rev.ToString());
            }
        }
        else
        {
            sb.AppendLine("No hay reseñas para este juego");
        }

        Program.SendResponse(networkDataHelper, sb.ToString());
    }
}