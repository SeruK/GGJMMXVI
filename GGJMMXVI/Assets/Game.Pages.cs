using UnityEngine;
using System.Collections;

public partial class Game : MonoBehaviour {
	private struct Tag {
		private readonly string value;

		public Tag( string value ) { this.value = value; }
		public static explicit operator string( Tag tag ) { return tag.value; }
	}

	private struct ButtonInfo {
		public readonly string text;
		public readonly string pageTag;
		public readonly bool forceIfFat;
		public readonly int kcal;
		public ButtonInfo( string text, bool forceIfFat = false ) {
			this.text = text;
			this.pageTag = null;
			this.kcal = 0;
			this.forceIfFat = forceIfFat;
		}
		public ButtonInfo( string text, string pageTag, bool forceIfFat = false ) {
			this.text = text;
			this.pageTag = pageTag;
			this.kcal = 0;
			this.forceIfFat = forceIfFat;
		}
		public ButtonInfo( string text, string pageTag, int kcal, bool forceIfFat ) {
			this.text = text;
			this.pageTag = pageTag;
			this.kcal = kcal;
			this.forceIfFat = forceIfFat;
		}
	}

	private struct FoodInfo {
		public readonly string name;
		public readonly int kcal;
		public FoodInfo( string name, int kcal ) {
			this.name = name;
			this.kcal = kcal;
		}
	}

	private abstract class Page {
		public Tag tag { get; protected set; }
	}

	private class NormalPage : Page {
		public readonly string mainText;
		public readonly ButtonInfo[] buttons;

		public NormalPage( string mainText, params ButtonInfo[] buttons ) {
			this.mainText = mainText;
			this.buttons = buttons;
		}

		public NormalPage( string mainText, params string[] buttonTexts ) {
			this.mainText = mainText;
			buttons = new ButtonInfo[ buttonTexts.Length ];
			for( int i = 0; i < buttons.Length; ++i ) {
				buttons[ i ] = new ButtonInfo( buttonTexts[ i ] );
			}
		}

		public NormalPage( Tag tag, string mainText, params ButtonInfo[] buttons ) {
			this.tag = tag;
			this.mainText = mainText;
			this.buttons = buttons;
		}
	}

	private class FoodPage : Page {
		public readonly string mainText;
		public readonly FoodInfo[] food;
		public readonly bool singleChoice;
		public readonly string buttonText;
		public readonly string eatMainText;
		public readonly string postEatButtonText;

		public FoodPage( string mainText, FoodInfo[] food, string buttonText, string eatMainText, string postEatButtonText ) {
			this.mainText = mainText;
			this.food = food;
			this.buttonText = buttonText;
			this.eatMainText = eatMainText;
			this.postEatButtonText = postEatButtonText;
		}

		public FoodPage( Tag tag, string mainText, FoodInfo[] food, string buttonText, string eatMainText, string postEatButtonText ) {
			this.tag = tag;
			this.mainText = mainText;
			this.food = food;
			this.buttonText = buttonText;
			this.eatMainText = eatMainText;
			this.postEatButtonText = postEatButtonText;
		}

		public FoodPage( Tag tag, string mainText, FoodInfo[] food, string buttonText, string eatMainText, string postEatButtonText, bool singleChoice ) {
			this.tag = tag;
			this.mainText = mainText;
			this.food = food;
			this.singleChoice = singleChoice;
			this.buttonText = buttonText;
			this.eatMainText = eatMainText;
			this.postEatButtonText = postEatButtonText;
		}
	}

	private class ThrowPage : Page {
		public readonly string mainText;
		public readonly ButtonInfo button;
		public readonly bool aThird;

		public ThrowPage( string mainText, ButtonInfo buttonInfo ) {
			this.mainText = mainText;
			this.button = buttonInfo;
			this.aThird = false;
		}

		public ThrowPage( Tag tag, string mainText, ButtonInfo buttonInfo ) {
			this.tag = tag;
			this.mainText = mainText;
			this.button = buttonInfo;
			this.aThird = false;
		}

		public ThrowPage( string mainText, ButtonInfo buttonInfo, bool aThird ) {
			this.mainText = mainText;
			this.button = buttonInfo;
			this.aThird = aThird;
		}

		public ThrowPage( Tag tag, string mainText, ButtonInfo buttonInfo, bool aThird ) {
			this.tag = tag;
			this.mainText = mainText;
			this.button = buttonInfo;
			this.aThird = aThird;
		}
	}

	private static readonly Page[] pages = new Page[] {
		new NormalPage( "",
			"Wake up" ),
		new NormalPage( "You wake up in your room. It's saturday noon.",
			"Get up" ),
		new NormalPage( "It's pretty chilly. The fuzz on your arms stand on edge. You consider shaving it.",
			"Pick up clothes" ),
		new NormalPage( "\"A good breakfast is the start of a good day.\"",
			"Leave room" ),
		new NormalPage( "Before leaving you stop before the mirror hanging on your door.\nYour reflection is a pudgy, pale mess in ill fitting clothes.",
			"Go to bathroom" ),
		new NormalPage( "Tossing your clothes on the bathroom floor, you get on the scale.\nA bit less since yesterday.",
			"Go to kitchen" ),
		new NormalPage( "You get dressed and go upstairs.\nYour (parents') kitchen is newly renovated.\nThis is your first breakfast in a while.",
			"Open fridge" ),
		new FoodPage( new Tag( "Fridge" ), "Inside is:",
			new FoodInfo[] {
				new FoodInfo( "An egg", kcal: 70 ),
				new FoodInfo( "Bacon", kcal: 345 ),
				new FoodInfo( "Grape fruit", kcal: 35 ),
				new FoodInfo( "Carrot juice", kcal: 68 ),
				new FoodInfo( "Bread", kcal: 100 ),
			}, "Eat breakfast",
			"You eat your breakfast alone.", "Freshen up" ),
		new NormalPage( "Your teeth are brown in places. They could use a good brushing.",
			new ButtonInfo( "Use toilet", pageTag: "ThrowUpBreakfast", forceIfFat: true ),
			new ButtonInfo( "Brush teeth", pageTag: "PostBrush" ) ),
		new ThrowPage( new Tag( "ThrowUpBreakfast" ),
			"Hunkering over the toilet, you put your fingers down your throat.\nYour breakfast eagerly leaves your body.",
			new ButtonInfo( "Continue" ) ),
		new NormalPage( new Tag( "BreakfastBrush" ), "Your teeth are brown in places. They could use a good brushing.",
			new ButtonInfo( "Brush teeth" ) ),
		new NormalPage( new Tag( "PostBrush" ), "White Now(TM) is definitely the scam of decade.\nYou remember you've promised to meet your friend at a coffee shop.",
			new ButtonInfo( "Meet friend", pageTag: "MeetFriend" ),
			new ButtonInfo( "Go to gym", pageTag:"Gym" ) ),
 
		new NormalPage( new Tag( "MeetFriend" ), "Just some coffee",
			new ButtonInfo( "Take bus" ) ),
		new NormalPage( "While riding the bus to your destination you pick up a newspaper.\nThere's an article about the latest diet fad.",
			"Continue reading" ),
		new FoodPage( "Your friend greets you with a big hug and a smile.\nThe coffee shop has a wide array of treats.",
			new FoodInfo[] {
				new FoodInfo( "Coffee", kcal: 2 ),
				new FoodInfo( "Milk", kcal: 80 ),
				new FoodInfo( "Muffin", kcal: 315 ),
				new FoodInfo( "Apple", kcal: 70 ),
			}, "Fika",
			"Both of you order and sit down by a table. Your friend goes on about their training and diet.\nWhile breaking your food into pieces, an endless stream of words pour out of their mouth.",
			"Continue" ),
		new NormalPage( "\"If only I had a body like yours I wouldn't have to think about this stuff!\"\nYou don't appreciate the sarcasm. You question coming here.",
			"Grunt" ),
		new NormalPage( "That coffee went right through you. You excuse yourself and visit the bathroom.\nYou turn on the sink.",
			new ButtonInfo( "Leave", pageTag: "BusFik" ),
			new ButtonInfo( "Use toilet", pageTag:"", forceIfFat: true ) ),
		new ThrowPage( "You go about your business.", new ButtonInfo( "Go home" ) ),
		new NormalPage( new Tag( "BusFik" ), "On the bus home your phone rings.",
			new ButtonInfo( "Answer" ) ),
		new NormalPage( "A recognizably cheery voice greets you.\n\"Hey, I forgot to tell you, my dinner is still on tonight. Can I count on you?\"",
			new ButtonInfo( "Leave excuse", pageTag: "PostOuting" ) ),

		new FoodPage( new Tag( "Gym" ), "Arriving at the gym, you go straight to the floor.\nHow much cardio do you want to do today?",
			new FoodInfo[] {
				new FoodInfo( "Easy", -320 ),
				new FoodInfo( "Medium", -500 ),
				new FoodInfo( "Tough", -600 ),
			}, "Cardio!",
			"You do your cardio, avoiding the resentful looks of the people around you.",
			"Continue", singleChoice: true ),
		new NormalPage( "Light-headed, but satisfied you prepare to leave the gym.\nA bowl of chips with a notice is sitting on a table in the lobby.\n\"You deserve it!\"",
			new ButtonInfo( "Leave", pageTag: "BusGym" ), new ButtonInfo( "Take chips", pageTag: "", kcal: 45, forceIfFat: false ) ),
		new NormalPage( "All that work for nothing.",
			new ButtonInfo( "Leave" ) ),
		new NormalPage( new Tag( "BusGym" ), "On the bus home your phone rings.",
			new ButtonInfo( "Answer" ) ),
		new NormalPage( "A somewhat grumpy voice greets you.\n\"Where were you? I waited for like an hour.\"",
			"Sorry" ),
		new NormalPage( "\"Anyway, my dinner is still on for tonight. Hope you'll show up for that at least.\"",
			new ButtonInfo( "Leave excuse", pageTag: "PostOuting" ) ),

		new NormalPage( new Tag( "PostOuting" ), "You really don't feel like you have the energy for socializing.\nYour friend sounds let down.\n\"Good bye then.\"",
			new ButtonInfo( "Go home" ) ),

		new NormalPage ( "Finally home. A repulsive smell hits your nostrils.",
			"Go to your room" ),
		new NormalPage( "Before you reach the safety of your room, your mother's voice calls from upstairs.\n\"Honey! You're just in time for dinner.\"",
			"Not hungry" ),
		new NormalPage( "Her voice sterns.\n\"It's your father's birthday, honey. Come to the dining room.\"",
			"Well, fuck" ),

		new FoodPage( new Tag( "Dinner" ), "The table is set with the nice china.\nYour stomach turns when you notice what's being served.",
			new FoodInfo[] {
				new FoodInfo( "Pork chop", kcal: 246 ),
				new FoodInfo( "Boiled potatoes", kcal: 136 ),
				new FoodInfo( "Cream sauce", kcal: 68 ),
				new FoodInfo( "Peas", kcal: 20 ),
			}, "Eat", eatMainText: "", postEatButtonText: "" ),
		new NormalPage( "Your mother's glance pierces you, a cruel grin on her lips.\n\"Have some more, honey.\"\nShe begins loading your plate with food." ,
			"Oh no" ),
		new NormalPage( "\"You're going to finish every last bit. You have to eat, honey.\"",
			"Blank stare" ),
		new NormalPage( "You place some porkshop in your mouth, chewing it fifty times.",
			"Chew" ),
		new NormalPage( "You place some potato mush in your mouth, chewing it fifty times.",
			"Chew" ),
		new NormalPage( "When no one's looking, you discreetly spit the food into your napkin.",
			"Spit" ),
		new NormalPage( "You eat the peas, making sure no sauce gets on them.",
			new ButtonInfo( "Eat", pageTag: "", kcal: 20, forceIfFat: false ) ),
		new NormalPage( "You carefully start mixing what's left together.\nAfter mushing the potatoes, you swirl it in with the sauce.",
			"Swirl" ),
		new NormalPage( "You leave blank spots on the plate so it looks empty.",
			"Continue" ),
		new NormalPage( "While your parents are distracted, you gather the remaing pork chop in a pile.\nYou push as much mush as possible to the pile. You cover it with your fork and knife.",
			"Ask to be excused" ),
		new NormalPage( "Hiding the napkin in your hands, you ask to be excused.\n\"Please, we hardly spend any time together nowadays.\"",
			"Not hungry, need to rest" ),
		new NormalPage( "Another one of her sick grins distorts her face.\nThe napkin with the food is burning in your clenched grip.\n\"Have some more food, sweetie. You're so small.\"",
			"Why does she have to be so mean?" ),
		new NormalPage( "\"At least stay for dessert. I made your favorite cake.\"\nWhy is she taunting me?\nYou decline.",
			"Freshen up" ),

		new NormalPage( new Tag( "FlushNapkin" ), "You go downstairs. The napkin is firmly grasped in your hand.\nYou lock the door and turn on the sink.",
			new ButtonInfo( "Use toilet" ) ),
		new NormalPage( new Tag( "AfterFlushNapkin" ), "You flush. Round and round the food-filled napkin dances.\nIt's mesmerizing.\nYou're proud.",
			new ButtonInfo( "Use scale" ) ),
		new NormalPage( "You undress and stand on the scale.\nSame as in the morning.\nYou are in control.",
			"Go to your room" ),

		new NormalPage( "You put on Breakfast at Tiffany's. It's warm undernearth the bedsheets.\nYou think about the dessert upstairs.",
			"Keep watching" ),
		new NormalPage( "Time passes. Dessert. The movie isn't helping.\nBe strong. Be in control. You don't need it.",
			"Keep watching" ),
		new NormalPage( "Tea might help.\nTea will help.",
			"Go to kitchen" ),

		new NormalPage( new Tag( "KitchenCake" ), "The newly renovated kitchen is empty.\nYour parents have gone out.",
			new ButtonInfo( "Open fridge" ), new ButtonInfo( "Open cupboard", pageTag: "KitchenCake", forceIfFat: false ) ),
		new FoodPage( new Tag( "FridgeCake" ), "You get a spoon. You open the fridge",
			new FoodInfo[] {
				new FoodInfo( "Cake", kcal: 880 ),
			}, "Eat", eatMainText: "", postEatButtonText: "" ),
		new NormalPage( "You eat one piece.",
			new ButtonInfo( "Eat", pageTag: "", kcal: 200, forceIfFat: false ) ),
		new NormalPage( "You eat another.",
			new ButtonInfo( "Eat", pageTag: "", kcal: 200, forceIfFat: false ) ),
		new NormalPage( "You eat another.",
			new ButtonInfo( "Eat", pageTag: "", kcal: 200, forceIfFat: false ) ),
		new NormalPage( "You eat the rest of the cake.",
			new ButtonInfo( "Eat", pageTag: "", kcal: 278, forceIfFat: false ) ),
		new NormalPage( "You drag your fingers along the plate and lick them.",
			new ButtonInfo( "Lick", pageTag: "", kcal: 2, forceIfFat: false ) ),
		new NormalPage( "You eat a package of cookies from the cupboard.",
			new ButtonInfo( "Sugar", pageTag: "", kcal: 700, forceIfFat: false ) ),
		new NormalPage( "You eat leftovers from the dinner straight from the fridge with your bare hands.",
			new ButtonInfo( "Fat", pageTag: "", kcal: 300, forceIfFat: false ) ),
		new NormalPage( "You feel sick.",
			"Freshen up" ),

		new NormalPage( "You almost slip while running down the stairs.\nThe bathroom door flies open.",
			"Cleanse yourself" ),
		new ThrowPage( new Tag( "Throw1" ), "You turn the sink on.\nYou stick your fingers down your throat.\nGet the poison out.",
			new ButtonInfo( "Cleanse yourself" ), aThird: true ),
		new ThrowPage( new Tag( "Throw2" ), "Fingers further down your throat.\nGet it all out.",
			new ButtonInfo( "Cleanse yourself" ), aThird: true ), 
		new ThrowPage( new Tag( "Throw3" ), "You keep going until barely anything comes out.\nYou cough, clinging to the toilet seat.\nYour throat hurts.",
			new ButtonInfo( "Use scale" ) ),
		new NormalPage( "You fat failure.",
			"Go to your room" ),

		new NormalPage( "You return to your room. Audrey Hepburn is having a party.\nShe's wearing a sleeveless dress.\nHer arms are beautiful.\nYou put your fingers around your own arm. The fingertips don't touch.",
			"Continue" ),
		new NormalPage( "You set a new goal.",
			"Fin" ),
	};
}
