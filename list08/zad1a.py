from selenium.webdriver import Chrome
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions
from selenium.webdriver.common.keys import Keys
from selenium.webdriver.common.by import By
from selenium.webdriver.support.select import Select

driver = Chrome()

driver.get("http://localhost:8000")

element = driver.find_element(By.CLASS_NAME,"topnav")
element =  element.find_element(By.LINK_TEXT,"Contact")
element.click()

form = driver.find_element(By.ID,"contact_form")

c = None
for e in [*form.find_elements(By.TAG_NAME,"input"),form.find_element(By.TAG_NAME,"textarea")]:
    if e.get_attribute("type") == "submit":
        c = e
        continue
    assert(len(e.text)==0) 
    e.send_keys("your value")


s = Select(form.find_element(By.TAG_NAME,"select"))
s.select_by_value('italy')


driver.save_screenshot("zad1.png")

c.click()

driver.switch_to.alert.accept()