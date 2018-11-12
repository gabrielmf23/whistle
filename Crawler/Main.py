import bs4 as bs
import urllib.request
import re
import datetime
from Match import Match
from Log import Log
from Functions import Functions


def decrypt_connection_data():
    return False


# initialize the log
log = Log(datetime.datetime.now())
functions = Functions(log)
_match = Match(log)

# load championship list
championships = functions.load_championships()
if championships == "":
    log.add("Error recovering the links.", 1)

for championship in championships:

    # clear the index
    i = 0

    # the majority of the analysed championships have less than 10 pages of matches
    # so I added this "limit"
    # this limit should be updated to the true limit page of the championship
    while i < 10:
        i += 1
        # access the round "i" of the championship
        try:
            sauce = urllib.request.urlopen(championship['Link'] + str(i))
        except Exception:
            log.add("Championship - Error accessing: " + championship['Link'] + str(i), 0)
            continue

        # converts the html to xml
        soup = bs.BeautifulSoup(sauce, 'lxml')

        # if is the "last page" of the champioship, change the index to finalize
        if soup.find('li', class_='next button_link') is None:
            i = 11

        # search the match's list
        matches = soup.find('table', class_=re.compile('competition-rounds')).find_all('tr', id=re.compile('gsm_id_'))

        for match in matches:

            # verify if it's a live match
            # if it's, shouldn't capture to avoid capture incomplete data
            if match.find('td', class_=re.compile('gameinlive')) is not None:
                continue

            # set the championship id
            _match.championship = championship['ChampionshipID']

            # get match's result
            _match.match_result = functions.check_result((match.find_all('a', target='_self'))[1].text)
            if _match.match_result == 'I':  # Ignore the record
                _match.match_result = ""
                continue

            # get the "short" name of teams
            _match.home_team_name = functions.amend_quotation_marks((match.find_all('a', target='_self'))[0].text)
            _match.away_team_name = functions.amend_quotation_marks((match.find_all('a', target='_self'))[2].text)

            # get the date
            _match.match_date = functions.adjust_match_date((match.find('td', class_='nowrap')).text)

            # 5 and 9 are the positions of the links, to access the registration pages of the teams, in the match tag
            _match.home_team_complete_name = functions.get_the_complete_name(
                match.contents[5].contents[1].attrs['href'])
            _match.away_team_complete_name = functions.get_the_complete_name(
                match.contents[9].contents[1].attrs['href'])

            # Access match data

            matchURL = ((match.find('td', class_=re.compile('darker'))).find('a')).get('href') + '/1/live/'
            try:
                sauce = urllib.request.urlopen(matchURL)
            except Exception:
                log.add("Match - Error accessing:" + matchURL, 0)
                continue

            soup = bs.BeautifulSoup(sauce, 'lxml')

            if (soup.find_all('table', id='linups'))[1].find('td', align='center').text.strip() == 'A':
                _match.referee = functions.amend_quotation_marks((soup.find_all('table', id='linups'))[1]
                                                                 .find('td', align='left').text.strip())
            else:
                _match.referee = "-"
                log.add("Referee not found! - " + matchURL, 0)

            # If is a next match, it won't have "events"
            # Or, if it is a cancelled/postponed match, should delete the record
            if _match.match_result == 'N' or _match.match_result == 'R':
                _match.serialize()
                continue

            # Access the events' table of the match, one table for each half-time, looking for yellow/red cards
            match_events_table = soup.find_all('table', id=re.compile('-half-summary'))
            period = 0
            for half_time in match_events_table:
                period = period + 1
                match_events_trs = (half_time.find('tbody', class_='stat-quarts-padding')).\
                    find_all('tr', class_=re.compile('event-'))
                for match_events_tr in match_events_trs:
                    match_events_td = match_events_tr.find_all('td', class_=re.compile('match'))

                    # For each event, there's a TR tag with 6 (six) TDs
                    # If the 1st TD has content, is an event of the Home Team
                    if len(match_events_td[1].contents) > 1:
                        # Check if the event is a goal
                        if str(match_events_td[1].contents[1]).find('Goal') > -1:
                            if period == 1:
                                _match.home_team_goals_1st_period += 1
                            else:
                                _match.home_team_goals_2nd_period += 1
                            _match.home_team_goals_total += 1

                        # Check if the event is a 1st yellow card or a direct red card
                        if str(match_events_td[1].contents[1]).find('card') > -1:
                            if str(match_events_td[1].contents[1]).find('Yellow') > -1:
                                if period == 1:
                                    _match.home_team_1st_yellow_card_1st_period += 1
                                else:
                                    _match.home_team_1st_yellow_card_2nd_period += 1
                                _match.home_team_1st_yellow_card_total += 1
                            else:
                                if period == 1:
                                    _match.home_team_red_card_1st_period += 1
                                else:
                                    _match.home_team_red_card_2nd_period += 1
                                _match.home_team_red_card_total += 1

                        # Check if the event is a 2nd yellow card
                        if str(match_events_td[1].contents[1]).find('2nd/RC') > -1:
                            if period == 1:
                                _match.home_team_2nd_yellow_card_1st_period += 1
                            else:
                                _match.home_team_2nd_yellow_card_2nd_period += 1

                    # If the 4th TD has content, is an event of the Away team
                    elif len(match_events_td[4].contents) > 1:
                        # Check if the event is a goal
                        if str(match_events_td[4].contents[1]).find('Goal') > -1:
                            if period == 1:
                                _match.away_team_goals_1st_period += 1
                            else:
                                _match.away_team_goals_2nd_period += 1
                            _match.away_team_goals_total += 1

                        # Check if the event is a 1st yellow card or a direct red card
                        if str(match_events_td[4].contents[1]).find('card') > -1:
                            # Check if it's an Yellow or Red one
                            if str(match_events_td[4].contents[1]).find('Yellow') > -1:
                                if period == 1:
                                    _match.away_team_1st_yellow_card_1st_period += 1
                                else:
                                    _match.away_team_1st_yellow_card_2nd_period += 1
                                _match.away_team_1st_yellow_card_total += 1
                            else:
                                if period == 1:
                                    _match.away_team_red_card_1st_period += 1
                                else:
                                    _match.away_team_red_card_2nd_period += 1
                                _match.away_team_red_card_total += 1

                        # Check if the event is a 2nd yellow card
                        if str(match_events_td[4].contents[1]).find('2nd/RC') > -1:
                            if period == 1:
                                _match.away_team_2nd_yellow_card_1st_period += 1
                            else:
                                _match.away_team_2nd_yellow_card_2nd_period += 1
                            _match.away_team_2nd_yellow_card_total += 1

            _match.serialize()
            _match = Match()

# add log to finalize the application
log.finish()
