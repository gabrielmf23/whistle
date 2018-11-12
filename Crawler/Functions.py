import re
import urllib.request

import bs4 as bs
import jsonpickle
import requests


class Functions:
    def __init__(self, _log):
        self.log = _log

    @staticmethod
    def load_championships():
        # api_token = 'your_api_token'
        api_url = 'https://whistleprojectapi2018.azurewebsites.net/api/Links'

        response = requests.get(api_url)
        if response.status_code == 200:
            return jsonpickle.decode(response.content)
        else:
            return ""

    @staticmethod
    def amend_quotation_marks(name):  # check if it need to be done here, or can be sent to the API
        # remove the quotation to avoid problems to insert in the database
        if name.find("'") > -1:
            corrected_name = re.sub("'", "", name)
            return corrected_name
        return name

    def get_the_complete_name(self, link):
        try:
            return self.amend_quotation_marks((bs.BeautifulSoup(urllib.request.urlopen(link), 'lxml').find(
                'h2', class_='boxed-header')).text.strip())
        except Exception:
            self.log.add("Problem to get the complete name of the club.", 0)
            return ''

    @staticmethod
    def check_result(result):
        # region Possible "results"
        # W - Home win
        # D - Draw
        # L - Away win
        # N - Next match
        # P - Postponed match:
        #       Must check if was already recorded as a Next match, if so, must update the record with new date
        # C - Cancelled match:
        #       Must check if was already recorded as a Next match, if so, must delete the record
        # endregion

        if result.strip() == '-':
            return 'I'  # Not valid result

        if result.strip() == 'P':
            return 'P'  # should update the record if was inserted as a "next match"

        if result.strip() == 'Cancelled':
            return 'C'   # should delete the record if was inserted as a "next match"

        if result.strip() == 'vs':
            return 'N'  # check if it must be saved as a next match

        result = (result.strip()).split('-')

        if int(result[0]) == int(result[1]):
            return "D"

        # the result is defined for the home team
        # so if the searched team is the away team, the result is "inverted"
        if int(result[0]) > int(result[1]):
            return "W"

        return "L"

    def adjust_match_date(self, date):
        date = (date.split(' - ')[0]).strip().split(".")
        date[1] = self.return_month(date[1])
        # adjust to pattern Y-M-D
        corrected_date = str(date[2]) + '-' + str(date[1]) + '-' + str(date[0])
        return corrected_date

    @staticmethod
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

