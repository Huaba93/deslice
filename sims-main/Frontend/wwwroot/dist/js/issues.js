  async function GetCveDetails(cve){
    let res = await fetch("https://services.nvd.nist.gov/rest/json/cves/2.0?cveId=" + cve);
    let data = await res.json();
    return data.vulnerabilities[0].cve;
  }

function CveChangeEvent(cveField){
  document.getElementById("progBar").style = "animation: borealisBar 20s linear infinite";
  const prom =  GetCveDetails(cveField.value);
prom.then(r => {
  document.getElementById("newIssueCvssBase").value = r.metrics.cvssMetricV31[0].cvssData.baseScore;
  document.getElementById("newIssueCvss").value = r.metrics.cvssMetricV31[0].cvssData.vectorString;
  document.getElementById("newIssueDesc").value = r.descriptions[0].value;
  document.getElementById("progBar").style = "";
});
}


function CacheInput(e){
  console.log("{\”"+e.name+"\”:\""+e.value+"\”}")
  
  $.ajax({
    type: "POST",
    url: "/Issues?handler=Redis",
    data: {
      __RequestVerificationToken: gettoken(),
      name : e.name,
      value : e.value
    },
    dataType: "json",
    success: function(){
    //success code here
    },
    error: function(){
    //error code here
    }
    });
}


//const responseData = Promise.resolve(prom).then(r => console.log(r.metrics));
 //console.log(responseData.metrics);
  // let cve = "CVE-2023-1234";
  // let cveDetails = fetch("https://services.nvd.nist.gov/rest/json/cves/2.0?cveId=" + cve)
  // .then(r => r.json())
  // .then(d => d.vulnerabilities[0].cve)
  // .then(l => console.log(l))
  // //.then(bs => console.log(bs.metrics.cvssMetricsV31[0].cvssData.baseScore))
  // //.then(cvss => console.log(cvss.metrics.cvssMetricsV31[0].cvssData.vectorString))
  // .then(desc => console.log(desc.description[0].value))
  // .finally();
