class Match:
    def __init__(self, referee, ht_name, ht_complete_name, ht_y_card, ht_r_card, aw_name, aw_complete_name, aw_y_card,
                 aw_r_card, match_result, match_date, championship):
        self.referee = referee
        self.ht_name = ht_name
        self.ht_complete_name = ht_complete_name
        self.ht_y_card = ht_y_card
        self.ht_r_card = ht_r_card
        self.aw_name = aw_name
        self.aw_complete_name = aw_complete_name
        self.aw_y_card = aw_y_card
        self.aw_r_card = aw_r_card
        self.match_result = match_result
        self.match_date = match_date
        self.championship = championship

        # ht stands for Home Team
        # aw stands for Away Team
        # y_card stands for Yellow Card
        # r_card stands for Red Card

    def empty_match(self):
        self.referee = ""
        self.ht_name = ""
        self.ht_complete_name = ""
        self.ht_y_card = 0
        self.ht_r_card = 0
        self.aw_name = ""
        self.aw_complete_name = ""
        self.aw_y_card = 0
        self.aw_r_card = 0
        self.match_result = ""
        self.match_date = ""
        self.championship = 0
