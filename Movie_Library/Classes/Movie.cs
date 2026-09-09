using System;

namespace Movie_Library.Classes
{

    /// <summary>
    /// Classe Movie qui représente un film.
    /// </summary>
	internal class Movie
	{
        /// <summary>
        /// L'id du film.
        /// </summary>
		private int _id;


        /// <summary>
        /// Le titre du film.
        /// </summary>
		private string _title;

        /// <summary>
        /// Le synopsis du film.
        /// </summary>
        private string _synopsis;

        /// <summary>
        /// Le poster du film.
        /// </summary
        private string _poster;

        /// <summary>
        /// Le score provenant de TMBD.
        /// </summary>
        private float _ratingTMDB;

       /// <summary>
       /// Le score personnel du l'utilisateur.
       /// </summary>
        private float _personalRating;  

        /// <summary>
        /// Les notes personnelles sur le film.
        /// </summary>
        private string _personalNote;

        /// <summary>
        /// Le statut de visionnage du film.
        /// </summary>
        private Status _status;

        /// <summary>
        /// Méthode pour récupérer le titre.
        /// </summary>
        /// <returns></returns>

        public string GetTitle()
        {
            return _title;
        }

        /// <summary>
        /// Méthode pour récupérer le synopsis
        /// </summary>
        /// <returns></returns>
        public string GetSynopsis()
        {
            return _synopsis;
        }

       /// <summary>
       /// Méthode pour récupérer le poster d'un film
       /// </summary>
       /// <returns></returns>
        public string GetPoster()
        {
           return _poster; 
        }

        /// <summary>
        /// Méthode pour récupérer le rating TMDB pour un film
        /// </summary>
        /// <returns></returns>
        public float GetRatingTMDB()

        {
           return _ratingTMDB; 
        }

        /// <summary>
        /// Méthode pour récupérer le score personnel d'un film de l'utilisateur 
        /// </summary>
        /// <returns></returns>

        public float GetPersonalRating()

        {
            return _personalRating;
        }

        /// <summary>
        /// Méthode pour récupérer les notes personelles de l'utilisateur d'un film
        /// </summary>
        /// <returns></returns>

        public string GetPersonalNote()

         {
            return _personalNote; 
            
        }

          /// <summary>
          /// Méthode pour récupérer le statut de visionnage d'un film
          /// </summary>

        public Status GetStatus()

        { 
           return _status; 
        } 
        
        /// <summary>
        /// Méthode pour mettre le score de l'utilisateur sur un film
        /// </summary>
        /// <param name="PersonalRating"></param>
        /// <returns></returns>

        public void SetPersonalRating  (float personalRating)
        { 
            _personalRating= personalRating; 
        }

        /// <summary>
        /// Méthode pour écrire des notes personnelles sur un film
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>

        public void SetPersonalNote(string note)
        {
            _personalNote = note;
        }


        /// <summary>
        /// Supprime les notes personnelles sur un film.
        /// </summary>

        public void RemovePersonalNote() 
        {
            if (!string.IsNullOrEmpty(_personalNote))
            {
                _personalNote = "";
                Console.WriteLine("La note a été supprimée.");
            }
            else
            {
                Console.WriteLine("Aucune note à supprimer.");
            }
            
        }

        /// <summary>
        ///  Modifie le statut de visionnage (seen, not seen, in progress)
        /// </summary>
        /// <param name="status"></param>
        public void ModifyStatus(Status status) 
        {
            if (_status != status)
            {
                _status = status;
                Console.WriteLine($"Le statut a été modifié : {status}.");
            }
            else
            {
                Console.WriteLine($"Le film est déjà au statut : {status}.");
            }
        }
		

	}
}
