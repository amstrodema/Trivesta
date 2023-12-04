var baseUrl = "https://localhost:44359";

function alertSuccess(val) {
    alertThis(val, "success");
}
function alertDanger(val) {
    alertThis(val, "danger");
}
function alertWarning(val) {
    alertThis(val, "warning");
}


function alertThis(val, type) {
    var div = document.createElement("div");
    div.innerText = val;
    div.classList.add("slide-in-left");
    div.classList.add("alertFloater");

    try {
        div.classList.remove("alertSuccess");
        div.classList.remove("alertWarning");
        div.classList.remove("alertDanger");
        div.classList.remove("hidden");
    } catch (e) {

    }

    if (type == "success") {
        div.classList.add("alertSuccess");
    }
    else if (type == "warning") {
        div.classList.add("alertWarning");
    }
    else if (type == "danger") {
        div.classList.add("alertDanger");
    }


    document.getElementById("alertMe").append(div)

    setTimeout(() => {
        div.classList.add("hidden");
    }, 3000)
}


function makePayment(pubKey, ref, amt, email, name) {
    FlutterwaveCheckout({
        public_key: pubKey,
        tx_ref: ref,
        amount: amt,
        currency: "NGN",
        payment_options: "card, banktransfer, ussd",
        //redirect_url: "https://trendycampus.com/funding/CreditWallet",
        redirect_url: baseUrl+"/manager/CreditWallet",
        meta: {
            source: "docs-inline-test",
            consumer_mac: "92a3-912ba-1192a",
        },
        customer: {
            email: email,
            name: name,
        },
        customizations: {
            title: "Trivesta Coins",
            description: "Purchase",
            logo: baseUrl+"/assets/images/logo.png",
        },
    });
}

function IsImageFile(fileInput) {
    try {
        fileName = fileInput.files[0].name;
        var imageExtensions = ['jpg', 'jpeg', 'png']; // Add more extensions if needed
        var extension = fileName.split('.').pop().toLowerCase();
        return imageExtensions.includes(extension);
    } catch (e) {
        return false;
    }
}
