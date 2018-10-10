import time
import datetime
import sys
import os


class Log:
    def __init__(self, start):
        self.start = start
        self.nome = "log_" + str(time.strftime('%d_%m_%y_-_%H_%M_%S')) + ".txt"
        if not os.path.exists("./Logs"):
            os.makedirs("./Logs")
        self.add("Started running!", 0)

    def add(self, message, code):
        f = open("Logs/" + self.nome, "a+")
        f.write(str(datetime.datetime.now()) + " - " + message + '\n')
        f.close()
        if code == 1:
            self.finish(self)

    def finish(self):
        total = datetime.datetime.now() - self.start
        self.add("Completed execution.", 0)
        self.add("Total execution time: " + str(total) + ".", 0)
        sys.exit()
