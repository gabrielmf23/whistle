import bs4 as bs
import urllib.request
import re
import jsonpickle
import requests
import datetime
from Match import Match
from Log import Log


def check_result(result):
    # check if it's a valid result
    if result.strip() == '-':
        return 'I'  # if it isn't, just ignore the match

    if result.strip() == 'P' or result.strip() == 'Cancelled':
        return 'P'  # should delete the record if inserted as a "next match"

    if result.strip() == 'vs':
        return 'N'  # check if it should be saved as a next match

    result = (result.strip()).split('-')

    if int(result[0]) == int(result[1]):
        return "D"

    # the result is defined for the home team
    # so if the searched team is the away team, the result is "inverted"
    if int(result[0]) > int(result[1]):
        return "W"

    return "L"


def amend_quotation_marks(name):  # check if it need to be done here, or can be sent to the API
    # remove the quotation to avoid problems to insert in the database
    if name.find("'") > -1:
        corrected_name = re.sub("'", "", name)
        return corrected_name
    return name


def adjust_match_date(date):
    date = (date.split(' - ')[0]).strip().split(".")
    date[1] = return_month(date[1])
    # adjust to pattern Y-M-D
    corrected_date = str(date[2]) + '-' + str(date[1]) + '-' + str(date[0])
    return corrected_date


def return_month(mes):
    if mes == "Jan":
        return 1
    if mes == "Fev":
        return 2
    if mes == "Mar":
        return 3
    if mes == "Abr":
        return 4
    if mes == "Mai":
        return 5
    if mes == "Jun":
        return 6
    if mes == "Jul":
        return 7
    if mes == "Ago":
        return 8
    if mes == "Set":
        return 9
    if mes == "Out":
        return 10
    if mes == "Nov":
        return 11
    return 12


def get_the_complete_name(link):
    try:
        return amend_quotation_marks((bs.BeautifulSoup(urllib.request.urlopen(link), 'lxml').find(
            'h2', class_='boxed-header')).text.strip())
    except Exception:
        log.add("Problem to get the complete name of the club.", 0)
        return ''


def decrypt_connection_data():
    return False


def serialize(match):
    # api_token = 'your_api_token'
    api_url = "http://localhost:51237/api/Matches"

    headers = {'Accept': 'application/json',
               'Content-Type': 'application/json'}
    # 'Authorization': 'Bearer {0}'.format(api_token)}

    try:
        serialized_object = jsonpickle.dumps(match)
    except TypeError as error:
        log.add(repr(error), 0)
        return None

    message = ""
    response = requests.post(api_url, headers=headers, json=serialized_object)

    if response.status_code == 200:
        if match.match_result != 'N' or match.match_result != 'R':
            log.add("Collected - " + match.ht_name + " x " + match.aw_name + " - on: " + match.match_date + ".", 0)
        elif match.championship == 'N':
            log.add("Collected - Next match: " + match.ht_name + " x " + match.aw_name + " - on: " + match.match_date +
                    ".", 0)
        else:
            log.add("Removed - Postponed/Cancelled match: " + match.ht_name + " x " + match.aw_name + " - on: " + match.match_date +
                    ".", 0)
    else:
        if response.status_code >= 500:
            message = "Server Error"
        elif response.status_code == 404:
            message = "Problem connecting to API"
        elif response.status_code == 401:
            message = "Authentication Failed"
        elif response.status_code >= 403:
            message = "Access forbidden"
        elif response.status_code >= 300:
            message = "Unexpected redirect"
        else:
            message = "Unexpected Error"

    if message != "":
        log.add("Error code: " + str(response.status_code) + " - " + message, 0)
    return None


def load_championships():
    # api_token = 'your_api_token'
    api_url = 'http://localhost:51237/api/Links'

    response = requests.get(api_url)
    if response.status_code == 200:
        return jsonpickle.decode(response.content)
    else:
        return ""


_match = Match("", "", "", 0, 0, "", "", 0, 0, "", "", 0)

# initialize the log
log = Log(datetime.datetime.now())

# load championship list
championships = load_championships()
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
            _match.match_result = check_result((match.find_all('a', target='_self'))[1].text)
            if _match.match_result == 'I':  # Ignore the record
                _match.match_result = ""
                continue

            # get the "short" name of teams
            _match.ht_name = amend_quotation_marks((match.find_all('a', target='_self'))[0].text)
            _match.aw_name = amend_quotation_marks((match.find_all('a', target='_self'))[2].text)

            # get the date
            _match.match_date = adjust_match_date((match.find('td', class_='nowrap')).text)

            # 5 and 9 are the positions of the links, to access the registration pages of the teams, in the match tag
            _match.ht_complete_name = get_the_complete_name(match.contents[5].contents[1].attrs['href'])
            _match.aw_complete_name = get_the_complete_name(match.contents[9].contents[1].attrs['href'])

            # cartoes e arbitragem

            matchURL = ((match.find('td', class_=re.compile('darker'))).find('a')).get('href') + '/1/live/'
            try:
                sauce = urllib.request.urlopen(matchURL)
            except Exception:
                log.add("Match - Error accessing:" + matchURL, 0)
                continue

            soup = bs.BeautifulSoup(sauce, 'lxml')

            if (soup.find_all('table', id='linups'))[1].find('td', align='center').text.strip() == 'A':
                _match.referee = amend_quotation_marks((soup.find_all('table', id='linups'))[1].find('td', align='left')
                                                       .text.strip())
            else:
                _match.referee = "-"

            # If is a next match, it won't have "events"
            # Or, if it is a cancelled/postponed match, should delete the record
            if _match.match_result == 'N' or _match.match_result == 'R':
                serialize(_match)
                continue

            # Access the events' table of the match, one table for each half-time, looking for yellow/red cards
            match_events_table = soup.find_all('table', id=re.compile('-half-summary'))
            for half_time in match_events_table:
                match_events_trs = (half_time.find('tbody', class_='stat-quarts-padding')). \
                    find_all('tr', class_=re.compile('event-'))
                for match_events_tr in match_events_trs:
                    match_events_td = match_events_tr.find_all('td', class_=re.compile('match'))

                    # For each event, there's a TR tag with 6 (six) TDs
                    # If the 1st TD has content, is an event of the Home Team
                    if len(match_events_td[1].contents) > 1:
                        # Check if the event is a card
                        if str(match_events_td[1].contents[1]).find('card') > -1:
                            # Check if it's an Yellow or Red one
                            if str(match_events_td[1].contents[1]).find('Yellow') > -1:
                                _match.ht_y_card += 1
                            else:
                                _match.ht_r_card += 1
                    # If the 4th TD has content, is an event of the Away team
                    elif len(match_events_td[4].contents) > 1:
                        # Check if the event is a card
                        if str(match_events_td[4].contents[1]).find('card') > -1:
                            # Check if it's an Yellow or Red one
                            if str(match_events_td[4].contents[1]).find('Yellow') > -1:
                                _match.aw_y_card += 1
                            else:
                                _match.aw_r_card += 1

            serialize(_match)
            _match.empty_match()

# add log to finalize the application
log.finish()
