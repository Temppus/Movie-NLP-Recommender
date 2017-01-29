/*
    EXAMPLE:
            <div name="unlikeButton" movieId="someIMDBID"></div>
            <div name="likeButton" movieId="someIMDBID"></div>

            Parent node must have attribute name="likeContainer" !
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
        $(this).text(notLikedText).prepend($('<i class="thumbs outline up icon"></i>'));

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
        $(this).text(likedText).prepend($('<i class="heart icon"></i>'));

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
            success: function (previewMovieArray) {

                var movieId = jqueryButton.attr("movieId");
                var likedButtonStr = "<div class='ui red button' name='unlikeButton' movieId='" + movieId + "'> <i class='heart icon'></i> " + likedText + " </div>";
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
            success: function (previewMovieArray) {

                var movieId = jqueryButton.attr("movieId");
                var likeButtonStr = "<div class='ui green button' name='likeButton' movieId='" + movieId + "'> <i class='thumbs outline up icon'></i> " + notLikedText + "</div>";
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