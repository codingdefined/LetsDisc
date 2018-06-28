// Taken From : https://github.com/benahm/EnhPageDown

$(document).ready(function() {

    Markdown.local = Markdown.local || {};

    var converter = Markdown.getSanitizingConverter();
    var editor = new Markdown.Editor(converter, null, null);
    editor.run();
    
    $('.wmd-input:not(.processed)').TextAreaResizer();    
});