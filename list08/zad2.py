from selenium import webdriver
from selenium.webdriver.chrome.options import Options
from selenium.webdriver.common.by import By

import requests
import os

options = Options() 
options.headless = True
with  webdriver.Chrome(options=options) as driver:
    driver.get("http://localhost:8000")

    element = driver.find_element(By.CLASS_NAME,"topnav")
    element =  element.find_element(By.LINK_TEXT,"Gallery")
    element.click()

    element = driver.find_element(By.CLASS_NAME,"row")
    os.makedirs("downloaded_images1", exist_ok=True)
    for e in element.find_elements(By.TAG_NAME,"img"):
        url = e.get_attribute('src')
        file = url.split('/')[-1]
        with  requests.get(url, stream=True) as res:
            with open(f"downloaded_images1/{file}", 'wb') as f:
                f.write(res.content)