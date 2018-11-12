import jsonpickle
import requests


class Match:
    def __init__(self, _log):
        self.log = _log

        # region Referee
        self.referee = ""
        # endregion

        # region Result
        self.match_result = ""
        # endregion

        # region Date
        self.match_date = ""
        # endregion

        # region Championship
        self.championship = 0
        # endregion

        # region Home team data
        # region Home team name
        self.home_team_name = ""
        self.home_team_complete_name = ""
        # endregion

        # region Home team goals
        self.home_team_goals_1st_period = 0
        self.home_team_goals_2nd_period = 0
        self.home_team_goals_total = 0
        # endregion

        # region Home team 1st yellow card
        self.home_team_1st_yellow_card_1st_period = 0
        self.home_team_1st_yellow_card_2nd_period = 0
        self.home_team_1st_yellow_card_total = 0
        # endregion

        # region Home team 2nd yellow card
        self.home_team_2nd_yellow_card_1st_period = 0
        self.home_team_2nd_yellow_card_2nd_period = 0
        self.home_team_2nd_yellow_card_total = 0
        # endregion

        # region Home team red card
        self.home_team_red_card_1st_period = 0
        self.home_team_red_card_2nd_period = 0
        self.home_team_red_card_total = 0
        # endregion
        # endregion

        # region Away Team Data
        # region Away team name
        self.away_team_name = ""
        self.away_team_complete_name = ""
        # endregion

        # region Away team goals
        self.away_team_goals_1st_period = 0
        self.away_team_goals_2nd_period = 0
        self.away_team_goals_total = 0
        # endregion

        # region Away team 1st yellow card
        self.away_team_1st_yellow_card_1st_period = 0
        self.away_team_1st_yellow_card_2nd_period = 0
        self.away_team_1st_yellow_card_total = 0
        # endregion

        # region Away team 2nd yellow card
        self.away_team_2nd_yellow_card_1st_period = 0
        self.away_team_2nd_yellow_card_2nd_period = 0
        self.away_team_2nd_yellow_card_total = 0
        # endregion

        # region Away team red card
        self.away_team_red_card_1st_period = 0
        self.away_team_red_card_2nd_period = 0
        self.away_team_red_card_total = 0
        # endregion
        # endregion

    def serialize(self):
        # api_token = 'your_api_token'
        api_url = "http://localhost:51237/api/Matches"

        headers = {'Accept': 'application/json',
                   'Content-Type': 'application/json'}
        # 'Authorization': 'Bearer {0}'.format(api_token)}

        try:
            serialized_object = jsonpickle.dumps(self)
        except TypeError as error:
            self.log.add(repr(error), 0)
            return None

        message = ""
        response = requests.post(api_url, headers=headers, json=serialized_object)

        if response.status_code == 200:
            if self.match_result != 'N' or self.match_result != 'R':
                self.log.add("Collected - " + self.home_team_name + " x " + self.away_team_name + " - on: " +
                             self.match_date + ".", 0)
            elif self.championship == 'N':
                self.log.add(
                    "Collected - Next match: " + self.home_team_name + " x " + self.away_team_name + " - on: " +
                    self.match_date + ".", 0)
            else:
                self.log.add("Removed - Postponed/Cancelled match: " + self.home_team_name + " x " +
                             self.away_team_name + " - on: " + self.match_date + ".", 0)
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
            self.log.add("Error code: " + str(response.status_code) + " - " + message, 0)
