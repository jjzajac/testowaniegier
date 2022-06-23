from re import I, L
from selenium.webdriver import Chrome
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.common.by import By
from selenium.webdriver.support.select import Select
from selenium.webdriver.support import expected_conditions as EC


class BasePage(object):
    def __init__(self, driver):
        self.driver = driver

    def is_title_matches(self):
        return False

    def screenshot(file):
        driver.save_screenshot(file)


class HomePage(BasePage):

    def is_title_matches(self):
        return "Home" in self.driver.title

    def click_contact_button(self):
        element = self.driver.find_element(By.LINK_TEXT, "Contact")
        element.click()
        return ContactPage(self.driver)


class ContactPageFormInput():
    def __init__(self, form) -> None:
        elements = [
            *form.find_elements(By.TAG_NAME, "input"),
            form.find_element(By.TAG_NAME, "textarea")
        ]
        self.submit = filter(lambda e: e.get_attribute(
            "type") == "submit", elements)
        self.elements = filter(lambda e: e.get_attribute(
            "type") != "submit", elements)

    def is_empty(self):
        return all(lambda x: (len(x.text)), self.elements)

    def set_inputs(self, v):
        for e in self.elements:
            e.send_keys("your value")

    def sumbit(self):
        self.sumbit.click()


class CPFSelect():
    def __init__(self,s):
        self.select = s

    def select_one(self):
        self.select.select_by_value('italy')

class ContactPageForm():
    def __init__(self, el):
        self.element = el

    def inputs(self):
        return ContactPageFormInput(self.el)

    def selects(self):
        return CPFSelect(Select(form.find_element(By.TAG_NAME, "select")))


class ContactPage(BasePage):
    def is_title_matches(self):
        return "Contact" in self.driver.title

    def get_form(self):
        form = self.driver.find_element(By.ID, "contact_form")
        return ContactPageForm(form)

    def check_alert(self):
        WebDriverWait(self.driver, 10).until(EC.alert_is_present())
        self.driver.switch_to.alert.accept()


with Chrome() as driver:
    driver.get("http://localhost:8000")

    mp = HomePage(driver)
    assert(mp.is_title_matches())

    cp = mp.click_contact_button()
    assert(cp.is_title_matches())

    form = cp.get_form()

    inp = form.inputs()

    assert(inp.is_empty())
    inp.set_inputs("some value")
    
    s = form.selects()
    s.select_one()

    cp.screenshot("zad1.png")

    inp.sumbit()
    cp.check_alert()
