// List of all movies in tyhe shopping cart
var shopCartArray = new Array();


// Present a purchased movie
class ShoppingItem
{
    constructor(id,title,price)
    {
        var count;
        this.id = id;
        this.title = title;
        this.price = price;
    }
}



// Hide the Shopping Cart Button at start-up
$(function () { $("#shopping-cart-table").hide(); });



// Toggle shopping cart
$(".movie-shop-shopping-cart-btn").on('click', function () {
    $("#shopping-cart-table-header-title").css("width", "300px");

    if ($(".movie-shop-shopping-cart-badge").text() != 0) {
        $("#shopping-cart-table").fadeToggle(1500, function () { });
    }
    else {
        $("#shopping-cart-table").html("<tr><td><b style='color:red;'>Your cart is empty</b></td></tr>");
        $("#shopping-cart-table").fadeToggle(1500, function () { });
    }
});



// Function to empty the Shopping Cart
function emptyShoppingCart() {
    var response = confirm("Do you want to empty the shopping cart?");

    if (response == true) { // Empty the shopping cart
        document.querySelector("table").innerHTML = "";
        $(document.querySelector("table")).hide();
        $(".movie-shop-shopping-cart-badge").css("background-color", "lightgray");
        $(".movie-shop-shopping-cart-badge").text(0);
        $(".movie-shop-shopping-cart-icon").css("color", "gray");
        shopCartArray = [];
    }
};


// Function for Checkout
function checkOut() {

    // Make a string of the shopping cart
    let ShoppingCartList = "";

    for (var i=0; i < shopCartArray.length; i++) {
        ShoppingCartList +=  shopCartArray[i].title + "+" + shopCartArray[i].count + "+" + shopCartArray[i].price +
            "+" + shopCartArray[i].count * shopCartArray[i].price + "!";
    }

    // Write the string into hidden form
    $(document.querySelector("#ShoppingList")).val(ShoppingCartList);
    console.log(document.querySelector("#SubmitShoppingList").getAttribute("hello"));

    // Create an autoclick even to POST the string to the Razor page
    var AutoClick = document.createEvent("MouseEvents");
    AutoClick.initMouseEvent("click", true, true, window, 1, 0, 0, 0, 0,false, false, false, false, 0, null);
    document.querySelector("#SubmitShoppingList").dispatchEvent(AutoClick);
}


// Handle click on Add Cart Button (Buy)
$(".movie-shop-buy-btn").on('click', function () {

    var cartBadgeValue = $(".movie-shop-shopping-cart-badge").text();

    if (cartBadgeValue == 0) {
        $(".movie-shop-shopping-cart-badge").css("background-color", "green");
        $(".movie-shop-shopping-cart-icon").css("color", "green");
    }
    cartBadgeValue++;
    cartBadgeValue = $(".movie-shop-shopping-cart-badge").text(cartBadgeValue);

    AddItemToShoppingCart($(this));

    var ShoppingCartArrayLength = shopCartArray.length;

    if (ShoppingCartArrayLength > 0) {
        $("#shopping-cart-table").html(); // remove the content first

        // Add the table header
        var ShoppingItem =
            "<thead id='shopping-cart-table-header' style='width:360px;'><tr>" +
            "<th id='shopping-cart-table-header-title'>Title</th>" +
            "<th id='shopping-cart-table-header-number'>Quantity</th>" +
            "<th id='shopping-cart-table-header-price'>Price</th>" +
            "<th id='shopping-cart-table-header-sum'>Sum</th>" +
            "</tr> </thead>";

        // Add the items in the Shopping Array to the body of the table
        for (let i = 0; i < ShoppingCartArrayLength; i++) {
            ShoppingItem = ShoppingItem + "<tr>" +
                "<td>" + shopCartArray[i].title + "</td>" +
                "<td>" + shopCartArray[i].count + "</td>" +
                "<td>" + shopCartArray[i].price + "</td>" +
                "<td>" + shopCartArray[i].price * shopCartArray[i].count + "</td>" +
                "</tr>";
        }

        let totalSum = 0;
        for (i = 0; i < shopCartArray.length; i++) {
            totalSum += (shopCartArray[i].count * shopCartArray[i].price);
        }

        ShoppingItem = ShoppingItem + "<tr><td> Total Sum</td><td></td><td>SEK</td><td>" + totalSum + "</td></tr>";
        ShoppingItem = ShoppingItem + "<tr><td colspan='4'> <div>" +
            "<span class='btn btn-danger' id='empty-shopping-cart-btn' style='min-width:120px;' onclick='emptyShoppingCart()'> Empty Shopping Cart</span>" +
            "<span class='pull-right btn btn-success check-out-btn' style='width:120px;' onclick='checkOut()'>Checkout</span>" +
            "</div></td></tr>";

        // Update the shopping cart
        $("#shopping-cart-table").html(ShoppingItem);
    }
 });



// Add one movie to the shopping list
function AddItemToShoppingCart(item) {

    // retrieve the item attributes.
    let movieFound = false;
    let movieFoundIndex = 0;
    var id = $(item).attr("id");
    var title = $(item).attr("name");
    var price = $(item).attr("price");
    let movie = new ShoppingItem(id, title, price);
    const shopCartArrayLength = shopCartArray.length;

    // Check whether the item is already in the list of Shopping Cart.
    // If the item is in the list of items in then check the count. 
    // If it is between 1 to 3 then OK. Otherwise alert the user.

    if (shopCartArrayLength == 0) {
        movie.count = 1;
        shopCartArray.push(movie);
    }
    else {
            for (let i = 0; i < shopCartArrayLength; i++) {
                if (shopCartArray[i].id == movie.id) {
                    movieFound = true;
                    movieFoundIndex = i;
                    break;
                }
        }

        if (Boolean(movieFound))
        {
            // If the movie is already in the Array
            if (shopCartArray[movieFoundIndex].count == 3) {
                    var cartBadgeValue = $(".movie-shop-shopping-cart-badge").text();
                    cartBadgeValue--;
                    $(".movie-shop-shopping-cart-badge").text(cartBadgeValue);

                    alert("For ordering quantities more than 3 pcs. of one movie please contact our customer service");
                }
                else {
                shopCartArray[movieFoundIndex].count++;
                }
            }
        else {
                if (isNaN(movie.count)) {
                    movie.count = 1;
                }
                else {
                    movie.count++;
                }
                shopCartArray.push(movie);
            }
        }
}


// -------------------------------------------- Check Out Page ---------------------------------------------------//
// Handling of Quantity on Checkout page
function ChangeQty(id) {
    let index = 0;
    var BtnId = "#" + id;
    console.log(BtnId);
    var btn = document.querySelector(BtnId);
    index = BtnId.indexOf('Plus'); console.log(index);
    if (index == -1) {
        index = BtnId.indexOf('Minus');
    }

    console.log(index);

    var btnQtyId = BtnId.substring(0, index) + "QTY";
    console.log(btnQtyId);
    var BtnQty = document.querySelector(btnQtyId);
    console.log(BtnQty.innerHTML);

    // Change the value of Quantity Button
    let Value = parseInt(BtnQty.innerText, 10);
    let QtyVal = 0; // To keep the current value of Quantity Button
    if (btn.innerText == "+") {
        if (Value >= 0 && Value < 3) {
            Value++;
            QtyVal = Value;
        }
    }
    else {
        if (Value > 0) {
            Value--;
            QtyVal = Value;
        }
    }

    BtnQty.innerText = Value;
    console.log("Value: " + Value);

    //Get the value of Price column
    console.log("Next --> " + $(BtnQty).next());
    console.log("Next Next --> " + $(BtnQty).next().next().html());
    var parent = $(BtnQty).parent().parent();
    var price = $(parent).next();
    var sum = $(parent).next().next();

    // Multiply the new quantity with the Price and update the Sum column
    var oldValue = parseInt($(sum).text(), 10);
    $(sum).text(parseInt($(BtnQty).text(), 10) * parseInt($(price).text(), 10));

    console.log("Next --> " + $(price).text());
    console.log("Next Next --> " + $(sum).html());

    // Update the Total Sum 
    var totalSum = document.querySelector("#totalsum");
    Value = parseInt($(totalSum).text(), 10) + parseInt($(sum).text(), 10) - oldValue;
    $(totalSum).text(Value);

    // Remove movie if count = 0
    if (QtyVal == 0) {

        var response = confirm("Do you want to remove the movie from your shopping list?");
        if (response == true) {
            console.log("Response " + response);
            $(parent).parent().hide();
        }
    }
}





