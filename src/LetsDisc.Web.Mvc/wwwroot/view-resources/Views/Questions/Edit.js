(function() {
    $(function() {
        var _questionService = abp.services.app.question;
        var _$form = $('#QuestionEditForm');
        _$form.find('input:first').focus();
        _$form.validate();

        _$form.find('button[type="submit"]').click(function (e) {
            e.preventDefault();

            // If the form is not valid, just return 
            if (!_$form.valid()) {
                return;
            }

            //serializeFormToObject is defined in main.js
            var question = _$form.serializeFormToObject(); 
            _questionService.editQuestion(question).done(function (q) {
                location.href = '/Questions/Details/' + q.question.id;
            });
        });
    });
})();