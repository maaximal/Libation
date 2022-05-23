﻿using DataLayer;
using Dinah.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibationWinForms
{
	internal class SeriesEntry : GridEntry
	{
		public override DateTime DateAdded => Children.Max(c => c.DateAdded);
		public override string ProductRating { get; protected set; }
		public override string PurchaseDate { get; protected set; }
		public override string MyRating { get; protected set; }
		public override string Series { get; protected set; }
		public override string Title { get; protected set; }
		public override string Length { get; protected set; }
		public override string Authors { get; protected set; }
		public override string Narrators { get; protected set; }
		public override string Category { get; protected set; }
		public override string Misc { get; protected set; }
		public override string Description { get; protected set; }
		public override string DisplayTags => string.Empty;

		public override LiberateStatus Liberate => _liberate;

		protected override Book Book => SeriesBook.Book;

		private SeriesBook SeriesBook { get; set; }

		private LiberateStatus _liberate = new LiberateStatus { IsSeries = true };
		public void setSeriesBook(SeriesBook seriesBook)
		{
			SeriesBook = seriesBook;
			_memberValues = CreateMemberValueDictionary();
			LoadCover();

			// Immutable properties
			{
				var childLB = Children.Cast<LibraryBookEntry>();
				int bookLenMins = childLB.Sum(c => c.LibraryBook.Book.LengthInMinutes);

				var myAverageRating = new Rating(childLB.Average(c => c.LibraryBook.Book.UserDefinedItem.Rating.OverallRating), childLB.Average(c => c.LibraryBook.Book.UserDefinedItem.Rating.PerformanceRating), childLB.Average(c => c.LibraryBook.Book.UserDefinedItem.Rating.StoryRating));
				var productAverageRating = new Rating(childLB.Average(c => c.LibraryBook.Book.Rating.OverallRating), childLB.Average(c => c.LibraryBook.Book.Rating.PerformanceRating), childLB.Average(c => c.LibraryBook.Book.Rating.StoryRating));


				Title = SeriesBook.Series.Name;
				Series = SeriesBook.Series.Name;
				Length = bookLenMins == 0 ? "" : $"{bookLenMins / 60} hr {bookLenMins % 60} min";
				MyRating = myAverageRating.ToStarString()?.DefaultIfNullOrWhiteSpace("");
				PurchaseDate = childLB.Min(c => c.LibraryBook.DateAdded).ToString("d");
				ProductRating = productAverageRating.ToStarString()?.DefaultIfNullOrWhiteSpace("");
				Authors = Book.AuthorNames();
				Narrators = Book.NarratorNames();
				Category = string.Join(" > ", Book.CategoriesNames());
			}
		}

		// These methods are implementation of Dinah.Core.DataBinding.IMemberComparable
		// Used by Dinah.Core.DataBinding.SortableBindingList<T> for all sorting
		public override object GetMemberValue(string memberName) => _memberValues[memberName]();

		private Dictionary<string, Func<object>> _memberValues { get; set; }

		/// <summary>
		/// Create getters for all member object values by name
		/// </summary>
		private Dictionary<string, Func<object>> CreateMemberValueDictionary() => new()
		{
			{ nameof(Title), () => Book.SeriesSortable() },
			{ nameof(Series), () => Book.SeriesSortable() },
			{ nameof(Length), () => Children.Cast<LibraryBookEntry>().Sum(c=>c.LibraryBook.Book.LengthInMinutes) },
			{ nameof(MyRating), () => Children.Cast<LibraryBookEntry>().Average(c=>c.LibraryBook.Book.UserDefinedItem.Rating.FirstScore()) },
			{ nameof(PurchaseDate), () => Children.Cast<LibraryBookEntry>().Min(c=>c.LibraryBook.DateAdded) },
			{ nameof(ProductRating), () => Children.Cast<LibraryBookEntry>().Average(c => c.LibraryBook.Book.Rating.FirstScore()) },
			{ nameof(Authors), () => string.Empty },
			{ nameof(Narrators), () => string.Empty },
			{ nameof(Description), () => string.Empty },
			{ nameof(Category), () => string.Empty },
			{ nameof(Misc), () => string.Empty },
			{ nameof(DisplayTags), () => string.Empty },
			{ nameof(Liberate), () => Liberate.BookStatus },
			{ nameof(DateAdded), () => DateAdded },
		};
	}
}
