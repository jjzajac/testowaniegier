from selenium import webdriver
from selenium.webdriver.common.by import By

with  webdriver.Chrome() as driver:
    driver.get("https://en.wikipedia.org")
    element = driver.find_element(By.ID, "searchInput")
    
    element.send_keys("python")
    element.submit()

    element = driver.find_element(By.ID, "mw-content-text")
    elements = element.find_elements(
        By.XPATH, "//div[@class='mw-parser-output']/ul")
    elements = [e.find_element(By.TAG_NAME, "a").get_attribute('href')
                for e in elements]

    for e in elements:
        driver.execute_script(f"window.open('{e}','_blank');")
        driver.switch_to.window(driver.window_handles[-1])
        element = driver.find_element(By.CLASS_NAME, "mw-parser-output")
        element = list(filter(lambda x: len(x.text) != 0,element.find_elements(By.TAG_NAME, 'p')))[0]
        print(driver.title)
        print(element.text)