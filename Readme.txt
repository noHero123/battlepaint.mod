this little mod filters the trade-room cards.

available commands are:
/trs or \trs + one of the following commands:
search/s + Cardname			:search in your cards of the card named "Cardname"
searchopponent/so + Cardname            :search in opponent cards for the card named "Cardname"
contains/c  + string                    :search in your cards for cards whose name contains "string" 
containsopponent/co + string		:search in opponent cards for cards whose name contains "string" 
rarity/ra + rare/r and/or uncommon/u and/or common/c		:search in your cards for cards which have the specified rarity
rarityopponent/rao + rare/r and/or uncommon/u and/or common/c
resource/re + energy/e and/or growth/g and/or order/o and/or decay/d 		:search in your cards for cards which have the specified resource-type
resourceopponent/reo + energy/e and/or growth/g and/or order/o and/or decay/d
clear/c 				:clears your and your opponents filter
clearself/cls    			:clears your filter
clearopponent/clo,
showall/sa				:show event the untradeable cards of you and your opponent
morethan3/mt3				:show only cards you have more than 3 times
morethan3opponent/mt3o

examples:
/trs s God Hand
/trs c god
/trs ra r uncommon (searches rare and uncommon cards)
/trs re growth o   (searches growth and order cards)
/trs c
/trs sa
/trs mt3

Notice:


-the command search ignores previous filters
-a command after search deletes the search-filter (if he targets the same player as the search command)

-if you type successively commands the filters (except of the search/s filter) work additive:
if you type:
/trs c o
/trs ra rare u

it shows all cards whose name contains an "o" AND which are rare or uncommon