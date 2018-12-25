var MvcTree = (function () {
    function MvcTree(element, options) {
        var tree = this;
        element = tree.closestTree(element);
        if (element.dataset.id) {
            return tree.instances[parseInt(element.dataset.id)].set(options || {});
        }

        tree.values = {};
        tree.element = element;
        tree.element.dataset.id = tree.instances.length;
        tree.ids = tree.element.querySelector('.mvc-tree-ids');
        tree.view = tree.element.querySelector('.mvc-tree-view');
        tree.readonly = element.classList.contains('mvc-tree-readonly');

        [].forEach.call(tree.ids.children, function (input) {
            tree.values[input.value] = input;
        });

        [].forEach.call(tree.view.children, function (branch) {
            tree.update(branch, true);
        });

        tree.instances.push(tree);
        tree.set(options || {});
        tree.bind();
    }

    MvcTree.prototype = {
        instances: [],

        closestTree: function (element) {
            var tree = element;

            if (!tree) {
                throw new Error('Tree element was not specified.');
            }

            while (tree.parentElement && !tree.classList.contains('mvc-tree')) {
                tree = tree.parentElement;
            }

            if (tree == document) {
                throw new Error('Tree can only be created from within mvc-tree structure.');
            }

            return tree;
        },
        set: function (options) {
            var tree = this;

            tree.readonly = options.readonly == null ? tree.readonly : options.readonly;

            if (tree.readonly) {
                tree.element.classList.add('mvc-tree-readonly');
            } else {
                tree.element.classList.remove('mvc-tree-readonly');
            }
        },

        uncheck: function (branch) {
            var tree = this;
            tree.uncheckNode(branch);

            [].forEach.call(branch.querySelectorAll('li'), function (node) {
                tree.uncheckNode(node);
            });

            var parent = branch.parentElement.parentElement;
            while (parent.tagName == 'LI') {
                tree.update(parent);

                parent = parent.parentElement.parentElement;
            }
        },
        check: function (branch) {
            var tree = this;
            tree.checkNode(branch);

            [].forEach.call(branch.querySelectorAll('li'), function (node) {
                tree.checkNode(node);
            });

            var parent = branch.parentElement.parentElement;
            while (parent.tagName == 'LI') {
                tree.update(parent);

                parent = parent.parentElement.parentElement;
            }
        },

        update: function (branch, recursive) {
            if (branch.lastElementChild.tagName == 'UL') {
                var tree = this;
                var checked = 0;
                var unchecked = 0;
                var children = branch.lastElementChild.children;

                [].forEach.call(children, function (node) {
                    var states = recursive ? tree.update(node, recursive) : node.classList;

                    if (states.contains('mvc-tree-undetermined')) {
                        return;
                    } else if (states.contains('mvc-tree-checked')) {
                        checked += 1;
                    } else {
                        unchecked += 1;
                    }
                });

                if (children.length == unchecked) {
                    branch.classList.remove('mvc-tree-checked');
                    branch.classList.remove('mvc-tree-undetermined');
                } else if (children.length == checked) {
                    branch.classList.add('mvc-tree-checked');
                    branch.classList.remove('mvc-tree-undetermined');
                } else {
                    branch.classList.add('mvc-tree-undetermined');
                }
            }

            return branch.classList;
        },
        uncheckNode: function (node) {
            node.classList.remove('mvc-tree-checked');
            node.classList.remove('mvc-tree-undetermined');

            if (node.dataset.id && this.values[node.dataset.id]) {
                this.ids.removeChild(this.values[node.dataset.id]);

                delete this.values[node.dataset.id];
            }
        },
        checkNode: function (node) {
            node.classList.add('mvc-tree-checked');
            node.classList.remove('mvc-tree-undetermined');

            if (node.dataset.id && !this.values[node.dataset.id]) {
                var input = document.createElement('input');
                input.name = this.element.dataset.for;
                input.value = node.dataset.id;
                input.type = 'hidden';

                this.values[node.dataset.id] = input;
                this.ids.appendChild(input);
            }
        },

        collapse: function (branch) {
            branch.classList.add('mvc-tree-collapsed');
        },
        expand: function (branch) {
            branch.classList.remove('mvc-tree-collapsed');
        },

        bind: function () {
            var tree = this;

            [].forEach.call(tree.element.getElementsByTagName('a'), function (node) {
                node.addEventListener('click', function (e) {
                    e.preventDefault();

                    if (!tree.readonly) {
                        var branch = this.parentElement;
                        if (branch.classList.contains('mvc-tree-checked')) {
                            tree.uncheck(branch);
                        } else {
                            tree.check(branch);
                        }
                    }
                });
            });

            [].forEach.call(tree.element.querySelectorAll('.mvc-tree-branch > i'), function (branch) {
                branch.addEventListener('click', function () {
                    var branch = this.parentElement;
                    if (branch.classList.contains('mvc-tree-collapsed')) {
                        tree.expand(branch);
                    } else {
                        tree.collapse(branch);
                    }
                });
            });
        }
    };

    return MvcTree;
}());
