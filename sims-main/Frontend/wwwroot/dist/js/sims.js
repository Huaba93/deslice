/**
 * AdminLTE Demo Menu
 * ------------------
 * You should not use this file in production.
 * This file is for demo purposes only.
 */

/* eslint-disable camelcase */

function getCookie(cname) {
    let name = cname + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let ca = decodedCookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}
function deleteAllCookies() {
    const cookies = document.cookie.split(";");

    for (let i = 0; i < cookies.length; i++) {
        const cookie = cookies[i];
        const eqPos = cookie.indexOf("=");
        const name = eqPos > -1 ? cookie.substr(0, eqPos) : cookie;
        document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:00 GMT";
    }
}

document.getElementById("username_field").innerText = getCookie("username")

function NotificationCenter(){
    /* TO add for messages: div: messagearea
  <div class="dropdown-divider"></div>
                          <a href="#" class="dropdown-item">
                              <i class="fas fa-envelope mr-2"></i> 3 new CVE Entries
                              <span class="float-right text-muted text-sm">3 mins</span>
                          </a> */
  notification = getCookie("notification")
  aNotificaiton = jQuery.parseJSON(notification)
  mc = document.getElementById("messageCount");
  mc.innerText = aNotificaiton.length;
  aNotificaiton.forEach((notification) => {
      console.log({ notification });
      console.log(notification.Title);
      const dom = document.getElementById("messagearea");
      let a = document.createElement("a")
      a.classList.add("dropdown-item");
      let i = document.createElement("i")
      i.className = "fas fa-envelope mr2"; 
      let span = document.createElement("span")
      span.className = "float-right text-muted text-sm";
      a.appendChild(i);
      i.insertAdjacentText("afterend",notification.Message);
      a.appendChild(span);
      dom.appendChild(a);
  });
  }

  NotificationCenter();

function Logout(){
    deleteAllCookies();
    window.location.href = "/"
}
/* TO add for messages: div: messagearea
<div class="dropdown-divider"></div>
                        <a href="#" class="dropdown-item">
                            <i class="fas fa-envelope mr-2"></i> 3 new CVE Entries
                            <span class="float-right text-muted text-sm">3 mins</span>
                        </a>


*/
