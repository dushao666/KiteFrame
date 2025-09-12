from selenium import webdriver
from selenium.webdriver.common.by import By
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.common.action_chains import ActionChains
from selenium.common.exceptions import TimeoutException, NoSuchWindowException
import time

driver = webdriver.Chrome()
driver.maximize_window()
wait = WebDriverWait(driver, 20)

def click_navigation_menu(driver):
    """处理导航菜单点击"""
    print("➤ 步骤 4: 展开导航菜单")
    max_retries = 3
    
    nav_xpath = """
    //div[contains(@class, 'Header__GlobalNavigationButton') 
          and not(contains(@class, 'Header__GlobalNavigationButton--opened'))]
    /span[@class='Header_GlobalNav_guide_text' 
          and contains(text(), '导航')]
    """
    
    for attempt in range(max_retries):
        try:
            nav_btn = WebDriverWait(driver, 15).until(
                EC.element_to_be_clickable((By.XPATH, nav_xpath))
            )
            
            driver.execute_script("arguments[0].scrollIntoViewIfNeeded(true);", nav_btn)
            ActionChains(driver).move_to_element(nav_btn).pause(0.3).click().perform()
            print(f"尝试 {attempt+1}/{max_retries}: 导航按钮已点击")
            
            WebDriverWait(driver, 5).until(
                EC.presence_of_element_located((By.CSS_SELECTOR, "div.Header__GlobalNavigationButton--opened"))
            )
            print("✓ 导航菜单已成功展开")
            return True
            
        except Exception as e:
            print(f"尝试 {attempt+1}/{max_retries} 失败: {str(e)}")
            if attempt == max_retries-1:
                raise
            time.sleep(1)
    
    return False

def click_job_menu(driver):
    """处理职位菜单点击"""
    print("➤ 步骤 5: 点击职位菜单")
    
    job_xpath = """
    //h3[@class='menuGroupTitle' 
          and contains(text(), '招聘管理系统')]
    /following-sibling::div[@class='menuGroupContainer']
    //a[contains(@class, 'menu-content') 
          and contains(text(), '职位')]
    """
    
    try:
        WebDriverWait(driver, 10).until(
            EC.visibility_of_element_located((By.CSS_SELECTOR, "div.functionMapWrapper.Show"))
        )
        
        job_element = WebDriverWait(driver, 15).until(
            EC.element_to_be_clickable((By.XPATH, job_xpath))
        )
        
        driver.execute_script("""
            const element = arguments[0];
            const viewPortHeight = Math.max(document.documentElement.clientHeight, window.innerHeight || 0);
            const elementTop = element.getBoundingClientRect().top;
            window.scrollBy(0, elementTop - (viewPortHeight / 2));
        """, job_element)
        
        driver.execute_script("arguments[0].click();", job_element)
        print("✓ 职位菜单已点击")
        
        print("✓ 已进入职位管理页面")
        return True
        
    except TimeoutException:
        if "job-management" in driver.current_url:
            print("✓ 通过URL验证进入职位页面")
            return True
        raise
    
def switch_to_job_iframe(driver):
    """增强版iframe切换函数"""
    print("➤ 切换至职位管理iframe")
    max_retries = 3
    iframe_locators = [
        (By.ID, "iTalentFrame"),
        (By.XPATH, "//iframe[contains(@src, 'Recruitment.MyJobView')]"),
        (By.TAG_NAME, "iframe")
    ]

    for attempt in range(max_retries):
        try:
            driver.switch_to.default_content()
            WebDriverWait(driver, 30).until(
                EC.presence_of_element_located((By.ID, "convoy-container"))
            )

            for locator_type, locator_value in iframe_locators:
                try:
                    iframe = WebDriverWait(driver, 15).until(
                        EC.presence_of_element_located((locator_type, locator_value))
                    )
                    break
                except:
                    continue
            else:
                raise Exception("所有iframe定位策略失败")

            driver.switch_to.frame(iframe)
            print("✓ 已进入iframe上下文")
            return True

        except Exception as e:
            print(f"尝试 {attempt+1} 失败: {str(e)}")
            if attempt == max_retries-1:
                raise
            time.sleep(2)
    
    return False

def click_ad_manage_in_iframe(driver):
    """在iframe内点击前往广告管理按钮"""
    print("➤ 步骤 6: 在iframe内点击广告管理按钮")
    
    try:
        btn_locator = (
            By.XPATH,
            "//div[contains(@class, 'recruit-jobad-btn-group')]"
            "//div[contains(@class, 'phoenix-button__content') "
            "and text()='前往广告管理']"
        )
        
        btn = WebDriverWait(driver, 20).until(
            EC.element_to_be_clickable(btn_locator)
        )
        
        for attempt in range(3):
            try:
                ActionChains(driver).move_to_element(btn).pause(0.3).click().perform()
                driver.switch_to.default_content()
                WebDriverWait(driver, 10).until(
                    lambda d: len(d.window_handles) > 1
                )
                print("✓ 新窗口检测成功")
                return True
                
            except TimeoutException:
                driver.switch_to.frame(driver.find_element(By.XPATH, "//iframe[contains(@src, 'Recruitment.MyJobView')]"))
                if attempt == 2:
                    driver.execute_script("arguments[0].click();", btn)
                    time.sleep(3)
                    driver.switch_to.default_content()
                    if len(driver.window_handles) > 1:
                        return True
                    raise Exception("点击后未打开新窗口")
        
        return False
        
    except Exception as e:
        print(f"❌ 广告管理点击失败: {str(e)}")
        raise
        
def switch_to_job_iframe0(driver):
    """增强版iframe切换函数 - 针对新页面结构优化"""
    print("➤ 切换至职位管理iframe")
    max_retries = 3
    
    for attempt in range(max_retries):
        try:
            driver.switch_to.default_content()
            WebDriverWait(driver, 30).until(
                EC.presence_of_element_located((By.ID, "convoy-container"))
            )
            
            iframe_locator = (By.XPATH, "//iframe[@id='iTalentFrame' and contains(@src, 'JobAd')]")
            iframe = WebDriverWait(driver, 30).until(
                EC.presence_of_element_located(iframe_locator)
            )
            driver.switch_to.frame(iframe)
            print("✓ 已进入iframe上下文")
            
            WebDriverWait(driver, 30).until(
                lambda d: d.execute_script("return document.readyState") == "complete"
            )
            
            WebDriverWait(driver, 20).until(
                EC.presence_of_element_located((By.CSS_SELECTOR, ".beisen-phoenix-tabs-super-tab"))
            )
            return True
            
        except Exception as e:
            print(f"尝试 {attempt+1} 失败: {str(e)}")
            if attempt == max_retries-1:
                raise
            time.sleep(2)
    
    return False

def click_recruitment_ad_tab(driver):
    """点击招聘门户广告标签 - 增强版"""
    print("➤ 步骤: 切换到招聘门户广告标签")
    try:
        tab_locator = (
            By.XPATH,
            "//div[contains(@class, 'beisen-phoenix-tabs-super-tab') and .//div[text()='招聘门户广告']]"
        )
        
        tab = WebDriverWait(driver, 30).until(
            EC.element_to_be_clickable(tab_locator)
        )
        
        for click_attempt in range(3):
            try:
                driver.execute_script("arguments[0].click();", tab)
                print("✓ 标签点击成功，状态已切换")
                return True
                
            except Exception as e:
                if click_attempt == 2:
                    if "aria-selected='true'" in tab.get_attribute("outerHTML"):
                        print("✓ 标签已处于选中状态")
                        return True
                    raise Exception("标签点击后未激活")
        
        return False
        
    except Exception as e:
        print(f"❌ 招聘门户广告标签点击失败: {str(e)}")
        raise

def open_advanced_filter_reliable(driver):
    """打开高级筛选面板 - 可靠版"""
    print("➤ 步骤: 打开高级筛选面板（可靠版）")
    try:
        switch_to_job_iframe(driver)
        
        locators = [
            (By.XPATH, "//span[contains(@class, 'phoenix-text-button') and contains(@class, 'phoenix-text-button--primary')]"),
            (By.XPATH, "//span[contains(@class, 'phoenix-text-button__textWraper') and text()='高级筛选']"),
            (By.XPATH, "//span[contains(@class, 'phoenix-text-button') and .//span[text()='高级筛选']]"),
            (By.XPATH, "//span[text()='高级筛选']")
        ]
        
        filter_btn = None
        for locator in locators:
            try:
                element = WebDriverWait(driver, 15).until(
                    EC.presence_of_element_located(locator)
                )
                if element.is_displayed():
                    filter_btn = element
                    break
            except:
                continue
        
        if not filter_btn:
            try:
                parent = driver.find_element(By.XPATH, "//div[contains(@class, 'nusion-metadata-table-filter')]")
                filter_btn = parent.find_element(By.XPATH, ".//span[text()='高级筛选']")
            except:
                raise Exception("所有定位器都无法找到高级筛选按钮")
        
        for click_attempt in range(3):
            try:
                driver.execute_script("arguments[0].click();", filter_btn)
                print("✓ 高级筛选面板已打开")
                return True
                
            except Exception as e:
                if click_attempt == 2:
                    try:
                        panel = driver.find_element(By.CLASS_NAME, "nusion-metadata-advanced-filter-panel")
                        if panel.is_displayed():
                            print("✓ 高级筛选面板已处于打开状态")
                            return True
                    except:
                        pass
                    raise Exception("高级筛选面板未打开")
                time.sleep(2)
        
        return False
        
    except Exception as e:
        print(f"❌ 打开高级筛选失败: {str(e)}")
        raise

def fill_creator_filter(driver, creator_name="董妍君"):
    """填写创建人筛选条件 - 精确版"""
    print(f"➤ 步骤: 填写创建人: {creator_name}")
    try:
        creator_container_xpath = "//div[@id='advanced-item-9557c2d2-85dc-45dc-a6f6-31f20dcc3dde']"
        creator_container = WebDriverWait(driver, 15).until(
            EC.visibility_of_element_located((By.XPATH, creator_container_xpath))
        )
        
        creator_input_xpath = "//div[@id='advanced-item-9557c2d2-85dc-45dc-a6f6-31f20dcc3dde']//input[@class='phoenix-select__input phoenix-select__input--unText']"
        creator_input = WebDriverWait(driver, 10).until(
            EC.element_to_be_clickable((By.XPATH, creator_input_xpath))
        )
        
        creator_input.click()
        creator_input.send_keys(creator_name)
        time.sleep(1)
        
        option_xpath = f"//div[contains(@class, 'phoenix-select-dropdown-item') and contains(., '{creator_name}')]"
        creator_option = WebDriverWait(driver, 10).until(
            EC.element_to_be_clickable((By.XPATH, option_xpath))
        )
        creator_option.click()
        return True
        
    except Exception as e:
        print(f"❌ 填写创建人失败: {str(e)}")
        raise

def apply_advanced_filter_robust(driver):
    """点击高级筛选的确定按钮（更健壮的定位方式）"""
    print("➤ 步骤: 点击高级筛选的确定按钮")
    try:
        confirm_button_xpath = "//div[contains(@class, 'phoenix-button__wraper--primary')]//div[text()='确定']"
        confirm_button = WebDriverWait(driver, 10).until(
            EC.element_to_be_clickable((By.XPATH, confirm_button_xpath))
        )
        confirm_button.click()
        time.sleep(2)
        print("✓ 筛选结果已加载")
        return True
    except Exception as e:
        print(f"❌ 点击确定按钮失败: {str(e)}")
        raise

def select_creator_dongyanjun(driver):
    """在人员选择器中搜索并选择'董妍君'"""
    print("➤ 步骤: 在人员选择器中搜索并选择'董妍君'")
    try:
        search_input_xpath = "//div[contains(@class, 'phoenix-search')]//input"
        search_input = WebDriverWait(driver, 15).until(
            EC.element_to_be_clickable((By.XPATH, search_input_xpath))
        )
        search_input.clear()
        search_input.send_keys("董妍君")
        time.sleep(1)
        
        dongyanjun_item_xpath = "//div[contains(@class, 'phoenix-w__staff-item-selectable') and .//span[text()='董妍君']]"
        dongyanjun_item = WebDriverWait(driver, 10).until(
            EC.element_to_be_clickable((By.XPATH, dongyanjun_item_xpath))
        )
        dongyanjun_item.click()
        time.sleep(0.5)
        
        selected_item_xpath = "//div[contains(@class, 'phoenix-user-select__depart-selected-items')]//div[.//span[text()='董妍君']]"
        WebDriverWait(driver, 10).until(
            EC.visibility_of_element_located((By.XPATH, selected_item_xpath))
        )
        
        confirm_button_xpath = "//div[contains(@class, 'phoenix-user-select__mode-button')]//div[contains(@class, 'phoenix-button__wraper--primary')]"
        confirm_button = WebDriverWait(driver, 10).until(
            EC.element_to_be_clickable((By.XPATH, confirm_button_xpath))
        )
        confirm_button.click()
        return True
        
    except Exception as e:
        print(f"❌ 选择董妍君失败: {str(e)}")
        raise

def click_creator_search_box(driver):
    """点击创建人搜索框以打开人员选择器"""
    print("➤ 步骤: 点击创建人搜索框")
    try:
        creator_search_box_xpath = "//*[@id='advanced-item-9557c2d2-85dc-45dc-a6f6-31f20dcc3dde']/div/div[2]/div/div/div/div/div/div"
        creator_search_box = WebDriverWait(driver, 15).until(
            EC.element_to_be_clickable((By.XPATH, creator_search_box_xpath))
        )
        creator_search_box.click()
        WebDriverWait(driver, 10).until(
            EC.visibility_of_element_located((By.CLASS_NAME, "phoenix-user-selector"))
        )
        return True
        
    except Exception as e:
        print(f"❌ 点击创建人搜索框失败: {str(e)}")
        raise

def select_all_jobs_robust(driver):
    """点击页面中的'全部勾选'复选框（更健壮的定位方式）"""
    print("➤ 步骤: 点击全部勾选复选框")
    try:
        select_all_xpath = "//div[contains(@class, 'phoenix-checkbox--noLabel')]//input[@type='checkbox']/ancestor::div[contains(@class, 'phoenix-checkbox')]"
        select_all_checkbox = WebDriverWait(driver, 15).until(
            EC.element_to_be_clickable((By.XPATH, select_all_xpath))
        )
        select_all_checkbox.click()
        time.sleep(0.5)
        return True
        
    except Exception as e:
        print(f"❌ 点击全部勾选复选框失败: {str(e)}")
        raise

def click_refresh_button_robust(driver):
    """点击页面中的刷新按钮（更健壮的定位方式）"""
    print("➤ 步骤: 点击刷新按钮")
    try:
        refresh_button_xpath = "//div[contains(@class, 'phoenix-button__wraper--primary')]//div[text()='刷新']"
        refresh_button = WebDriverWait(driver, 15).until(
            EC.element_to_be_clickable((By.XPATH, refresh_button_xpath))
        )
        refresh_button.click()
        WebDriverWait(driver, 10).until(
            EC.invisibility_of_element_located((By.CLASS_NAME, "phoenix-loading"))
        )
        return True
        
    except Exception as e:
        print(f"❌ 点击刷新按钮失败: {str(e)}")
        raise

def click_yes_button_robust(driver):
    """点击页面中的'是'按钮（更健壮的定位方式）"""
    print("➤ 步骤: 点击'是'按钮")
    try:
        yes_button_xpath = "//div[contains(@class, 'phoenix-button__wraper--primary')]//div[text()='是']"
        yes_button = WebDriverWait(driver, 15).until(
            EC.element_to_be_clickable((By.XPATH, yes_button_xpath))
        )
        yes_button.click()
        WebDriverWait(driver, 5).until(
            EC.invisibility_of_element_located((By.CLASS_NAME, "phoenix-modal"))
        )
        return True
        
    except Exception as e:
        print(f"❌ 点击'是'按钮失败: {str(e)}")
        raise
    
def execute_refresh_cycle(driver, cycles=5, interval=3):
    """执行刷新操作的循环"""
    print(f"\n=== 开始执行刷新循环 ({cycles}次) ===")
    
    for i in range(1, cycles + 1):
        print(f"\n➤ 循环 {i}/{cycles}")
        
        if not switch_to_job_iframe(driver):
            driver.switch_to.default_content()
            if not switch_to_job_iframe(driver):
                raise Exception("无法恢复iframe上下文")
        
        if not select_all_jobs_robust(driver):
            print("⚠️ 全选操作失败，尝试继续")
        
        if not click_refresh_button_robust(driver):
            print("⚠️ 刷新操作失败，尝试继续")
        
        if not click_yes_button_robust(driver):
            print("⚠️ 确认操作失败，尝试继续")
        
        time.sleep(interval)
    
    print(f"✅ 已完成 {cycles} 次刷新循环")

try:
    print("正在打开网站...")
    driver.get("https://www.italent.cn/")
    
    print("等待登录表单...")
    username = wait.until(EC.visibility_of_element_located((By.CSS_SELECTOR, "input#form-item-account")))
    password = wait.until(EC.visibility_of_element_located((By.CSS_SELECTOR, "input#form-item-password")))
    
    username.send_keys("dongyanjun@hisense.com")
    password.send_keys("hisense1.")
    print("用户名和密码已输入")
    
    print("处理隐私协议...")
    checkbox_box = wait.until(EC.element_to_be_clickable(
        (By.CSS_SELECTOR, "span.phoenix-checkbox__box")
    ))
    
    checkbox_input = checkbox_box.find_element(By.CSS_SELECTOR, "input.phoenix-checkbox__input")
    real_input = checkbox_box.find_element(By.CSS_SELECTOR, "span.phoenix-checkbox__realInput")
    
    is_checked = (
        checkbox_input.get_attribute("value") == "true" or 
        "phoenix-checkbox__realInput--checked" in real_input.get_attribute("class")
    )
    
    if not is_checked:
        checkbox_box.click()
        time.sleep(0.5)
    
    print("点击登录按钮...")
    login_btn = wait.until(EC.element_to_be_clickable(
        (By.XPATH, "//div[contains(@class, 'phoenix-button__content') and text()='登录']")
    ))
    driver.execute_script("arguments[0].click();", login_btn)

    # 4. 进入招聘管理系统
    if not click_navigation_menu(driver):
        raise Exception("无法展开导航菜单")

    # 5. 点击左侧职位菜单
    if not click_job_menu(driver):
        raise Exception("无法进入职位页面")

    # 6. 前往广告管理
    if not switch_to_job_iframe(driver):
        raise Exception("无法进入职位管理iframe")
    
    if not click_ad_manage_in_iframe(driver):
        raise Exception("进入广告管理失败")
    
    print("等待新窗口打开...")
    wait.until(lambda d: len(d.window_handles) == 2)
    driver.switch_to.window(driver.window_handles[1])
    
    WebDriverWait(driver, 30).until(
        EC.presence_of_element_located((By.TAG_NAME, "body"))
    )
    
    # 7. 操作招聘门户广告
    if not switch_to_job_iframe(driver):
        raise Exception("无法进入广告管理iframe")
    
    if not click_recruitment_ad_tab(driver):
        raise Exception("无法切换到招聘门户广告标签")
    
    # 8. 高级筛选
    print("打开高级筛选...")
    if not switch_to_job_iframe(driver):
        raise Exception("无法进入广告管理iframe")
    
    open_advanced_filter_reliable(driver)
    
    # 9. 输入创建人
    print("输入创建人...")
    switch_to_job_iframe(driver)
    click_creator_search_box(driver)
    select_creator_dongyanjun(driver)
    
    # 10. 点击两个确定按钮
    print("确认筛选条件...")
    apply_advanced_filter_robust(driver)
    time.sleep(1)
    
    # 执行循环操作
    execute_refresh_cycle(driver, cycles=3, interval=3)
    print("✅ 所有操作成功完成！")
    
    # 保持浏览器打开
    while True:
        time.sleep(5)

except (TimeoutException, NoSuchWindowException) as e:
    print(f"❌ 操作失败: {str(e)}")
    print("当前URL:", driver.current_url)
    print("窗口句柄:", driver.window_handles)
    
finally:
    driver.quit()