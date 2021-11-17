// List of all movies in tyhe shopping cart
var shopCartArray = new Array();


// Present a purchased movie
class ShoppingItem
{
    constructor(id,title,price)
    {
        var count;
        var alreadyInShoppingCart = false;
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


function emptyShoppingCart() {
    var response = confirm("Do you want to empty the shopping cart?");

    if (response == true) { // Empty the shopping cart
        document.querySelector("table").innerHTML = "";
        $(document.querySelector("table")).hide();
        $(".movie-shop-shopping-cart-badge").css("background-color", "lightgray");
        $(".movie-shop-shopping-cart-badge").text(0);
        $(".movie-shop-shopping-cart-icon").css("color", "gray");
        shopCartArray = [];

        //for (var i = 0; i < shopCartArray.length; i++) {
        //    shopCartArray[i]=shift();
        //}
    }
};



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
            "<thead class='thead-dark'><tr id='shopping-cart-table-header'>" +
            "<th id='shopping-cart-table-header-title' colspan='3'>Title</th>" +
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
            "<a class='pull-right btn btn-success check-out-btn' style='width:120px;' href='#'>Checkout</a>" +
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