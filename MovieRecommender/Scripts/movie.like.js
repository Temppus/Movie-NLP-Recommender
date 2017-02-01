/*
    Parent node must have attribute name="likeContainer" !

    EXAMPLE:
        <div name="likeContainer">
            <div name="unlikeButton" movieId="someIMDBID"></div>    --> Optional attributes for custom button texts (likeText="Like" likedText="Liked")
            <div name="likeButton" movieId="someIMDBID"></div>      --> Optional attributes for custom button texts (likeText="Like" likedText="Liked")
        </div>
*/


var debounceDelay = 200;

function likeDecorator() {

    var likedText = 'You liked this movie';
    var notLikedText = 'Like this movie ?';

    // decorate like div holder
    $('div[name=likeButton]').each(function () {

        if ($(this).attr("decorated") == "true")
            return;

        $(this).attr("decorated", "true");
        $(this).attr("class", "ui green button");

        var _notLikedText = $(this).attr("likeText") || notLikedText;

        $(this).text(_notLikedText).prepend($('<i class="thumbs outline up icon"></i>'));

        $(this).bind("click", $.debounce(debounceDelay,
            function () {
                likeFunction($(this));
            }
        ));
    });

    // decorate unlike div holder
    $('div[name=unlikeButton]').each(function () {

        if ($(this).attr("decorated") == "true")
            return;

        $(this).attr("decorated", "true");
        $(this).attr("class", "ui red button");

        var _likedText = $(this).attr("likedText") || likedText;

        $(this).text(_likedText).prepend($('<i class="heart icon"></i>'));

        $(this).bind("click", $.debounce(debounceDelay,
            function () {
                unlikeFunction($(this));
            }
        ));
    });

    var likeFunction = function (jqueryButton) {

        var movieId = jqueryButton.attr("movieId");

        if (movieId == null)
            return;

        var likeObject = {
            'IsLike': true,
            'IMDbId': movieId
        };

        likeObject = JSON.stringify({ model: likeObject });

        $.ajax({
            type: 'POST',
            url: '/Movie/LikeHandler',
            data: likeObject,
            contentType: 'application/json',
            error: function (err) {
                alert("Error, cannot like movie.");
            },
            success: function (data) {

                var movieId = jqueryButton.attr("movieId");
                var _likedText = jqueryButton.attr("likedText") || likedText;
                var likedButtonStr = "<div class='ui red button' name='unlikeButton' movieId='" + movieId + "'> <i class='heart icon'></i> " + _likedText + " </div>";
                var likedButton = $($.parseHTML(likedButtonStr));

                likedButton.click($.debounce(debounceDelay,
                                function () {
                                    unlikeFunction($(likedButton));
                                }
                    ));

                var likeContainer = $(jqueryButton).closest('[name=likeContainer]');
                jqueryButton.detach();
                likeContainer.append(likedButton);
            }
        });
    };

    var unlikeFunction = function (jqueryButton) {

        var movieId = jqueryButton.attr("movieId");

        if (movieId == null)
            return;

        var unlikeObject = {
            'IsLike': false,
            'IMDbId': movieId
        };

        likeObject = JSON.stringify({ model: unlikeObject });

        $.ajax({
            type: 'POST',
            url: '/Movie/LikeHandler',
            data: likeObject,
            contentType: 'application/json',
            error: function (err) {
                alert("Error, cannot unlike movie.");
            },
            success: function (data) {

                var movieId = jqueryButton.attr("movieId");
                var _notLikedText = jqueryButton.attr("likeText") || notLikedText;
                var likeButtonStr = "<div class='ui green button' name='likeButton' movieId='" + movieId + "'> <i class='thumbs outline up icon'></i> " + _notLikedText + "</div>";
                var likeButton = $($.parseHTML(likeButtonStr));

                likeButton.click($.debounce(debounceDelay,
                                function () {
                                    likeFunction($(likeButton));
                                }
                    ));

                var likeContainer = $(jqueryButton).closest('[name=likeContainer]');
                jqueryButton.detach();
                likeContainer.append(likeButton);
            }
        });
    };

};