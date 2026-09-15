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
        /// Constructeur de la classe Movie
        /// </summary>
        /// <param name="id">L'id du film.</param>
        /// <param name="title">Le titre du film.</param>
        /// <param name="synopsis">Le synopsis du film.</param>
        /// <param name="poster">Le poster du film.</param>
        /// <param name="ratingTMBD">Le score TMDB.</param>
        /// <param name="status">Le statut de visionnage initial.</param>
        public Movie (int id, string title, string synopsis, string poster, float ratingTMBD, Status status)
        {
            _id = id;
            _title = title;
            _synopsis = synopsis;
            _poster = poster;
            _ratingTMDB = ratingTMBD;
            _status = status;

            // Initialisation des données personnelles par défaut.
            _personalRating = 0;
            _personalNote = "";
        }

       
        /// <summary>
        /// Lecture seule du titre du film.
        /// </summary>
        /// <returns></returns>
        public string Title
        {
           get { return _title; }
        }

        /// <summary>
        /// Lecture seule de l'identifiant du film.
        /// </summary>
        public int Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Lecture seule du synposis du film.
        /// </summary>
        /// <returns></returns>
        public string Synopsis
        {
            get{ return _synopsis; }
        }

       /// <summary>
       /// Lecture seule du poster du film.
       /// </summary>
       /// <returns></returns>
        public string Poster
        {
           get { return _poster; }
        }

        /// <summary>
        /// Lecture seule du score TMDB.
        /// </summary>
        /// <returns></returns>
        
        public float RatingTMDB

        {
           get{ return _ratingTMDB; }
        }

        
        /// <summary>
        /// Lecture et modification du score personnel de l'utilisateur.
        /// </summary>
        /// <returns></returns>
        public float PersonalRating

        {
            get { return _personalRating; }
            set { _personalRating = value; }
        }

       /// <summary>
       /// Lecture et modification de la note personnel de l'utilisateur. 
       /// </summary>
       /// <returns></returns>
        public string PersonalNote

         {
            get { return _personalNote; }
            set { _personalNote = value; }
            
        }

         

        /// <summary>
        /// Lecture et modification du statut de visionnage.
        /// </summary>
        /// <returns></returns>
        public Status Status

        { 
           get { return _status; }
           set 
            {
                if (_status != value)
                {
                    _status = value;
                    Console.WriteLine($"Le statut a été modifié : {value}.");
                }
                else
                {
                    Console.WriteLine($"Le film est déjà au statut : {value}.");
                }

            }
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

	}
}
